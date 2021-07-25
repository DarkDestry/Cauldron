using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Cauldron.Core.Logging;
using Cauldron.IPC;
using Application = System.Windows.Application;

namespace Cauldron.Core
{
    public static class EtherManager
    {
        private const string ETHER_START_ARGUMENTS = "-sourceshaders";

        private static Process etherProcess;

        public static void Initialize(){
        
            string workingDir = CommandArguments.TryGetArgument("wd", out string path) ? path : AppContext.BaseDirectory;
            string executableName = CommandArguments.TryGetArgument("bin", out string exeName) ? exeName : "Ether.exe";

            ProcessStartInfo etherProcessStartInfo = new ProcessStartInfo(
                Path.Combine(workingDir, executableName), ETHER_START_ARGUMENTS);
            etherProcessStartInfo.WorkingDirectory = workingDir;
            
            etherProcess = Process.Start(etherProcessStartInfo);

            StartLogParser();
            ConnectionManager.Connect();
        }

        public static void Shutdown()
        {
            ConnectionManager.Close();
            StopLogParser();
            if (!etherProcess.HasExited) etherProcess.Kill();
        }



        private static Thread logParserThread;
        private static bool logParserThreadKeepAlive;

        private static void StartLogParser()
        {
            logParserThread = new Thread(() =>
            {
                string workingDir = CommandArguments.TryGetArgument("wd", out string path) ? path : AppContext.BaseDirectory;
                string latestLogFilePath = "";
                while (logParserThreadKeepAlive)
                {
                    latestLogFilePath = Directory.GetFiles(Path.Combine(workingDir, "logs"))
                        .Where(e => File.GetCreationTime(e) > etherProcess.StartTime)
                        .OrderByDescending(e => File.GetCreationTime(e).ToFileTimeUtc())
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(latestLogFilePath) && File.Exists(latestLogFilePath)) break;
                }

                if (!logParserThreadKeepAlive) return;

                var wh = new AutoResetEvent(false);
                var fsw = new FileSystemWatcher(Path.Combine(workingDir, "logs"));
                fsw.Filter = Path.GetFileName(latestLogFilePath);
                fsw.EnableRaisingEvents = true;
                fsw.Changed += (s,e) => wh.Set();

                var fs = new FileStream(latestLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs))
                {
                    var s = "";
                    while (logParserThreadKeepAlive)
                    {
                        s = sr.ReadLine();
                        if (s != null)
                            Application.Current.Dispatcher.Invoke(() => ParseEtherLogLine(s));
                        else
                            wh.WaitOne(1000);
                    }
                }

                fsw.EnableRaisingEvents = false;
                wh.Close();
            });
            logParserThreadKeepAlive = true;
            logParserThread.Start();
        }

        private static void StopLogParser()
        {
            logParserThreadKeepAlive = false;
            if (logParserThread.IsAlive) logParserThread.Join();
        }

        private static void ParseEtherLogLine(string logLine)
        {
            Match m = Regex.Match(logLine, @"\[ (?<timestamp>[^\]]+?) \] \[ +(?<module>[^\]]+?) +\] +(?<level>[^-]+?) - (?<message>.+)");
            if (!m.Success) Logger.LogError("Cauldron", $"Error parsing Ether LogLine: {logLine}");
            else
            {
                string message = $"[{m.Groups["module"].Value}] {m.Groups["message"].Value}";
                switch (m.Groups["level"].Value)
                {
                    case "INFO":
                        Logger.LogInfo("Ether", message);
                        break;
                    case "FATAL":
                        Logger.LogCritical("Ether", message);
                        break;
                    case "WARN":
                        Logger.LogWarning("Ether", message);
                        break;
                    case "ERROR":
                        Logger.LogError("Ether", message);
                        break;
                }
            }
        }
    }
}
