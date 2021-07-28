using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Cauldron.Core.Logging
{
    public static class Logger
    {
        private static ObservableCollection<LogEntry> _logEntries;

        private static ObservableCollection<LogEntry> LogEntries
        {
            get {
                if (_logEntries == null)
                {
                    Application.Current.Dispatcher.Invoke(() => _logEntries = new ObservableCollection<LogEntry>());
                }

                return _logEntries;
            }
            set => _logEntries = value;
        }

        private static LogLevel _logLevelMask = LogLevel.Critical | LogLevel.Error | LogLevel.Warning | LogLevel.Info | LogLevel.Debug;
        public static LogLevel LogLevelMask
        {
            get => _logLevelMask;
            set
            {
                _logLevelMask = value;
                filteredLogEntries.Refresh();
            }
        }

        private static string[] _sourcesMask = {"Cauldron", "Ether"};
        public static string[] SourcesMask
        {
            get => _sourcesMask;
            set
            {
                _sourcesMask = value;
                filteredLogEntries.Refresh();
            }
        }

        public static ICollectionView filteredLogEntries {
            get
            {
                ICollectionView source = null;
                Application.Current.Dispatcher.Invoke(() => source = CollectionViewSource.GetDefaultView(LogEntries));
                source.Filter = o => FilterLogEntries((LogEntry)o, _logLevelMask, _sourcesMask);
                return source;
            }
        }

        public static void LogCritical(string message) => Log(message, "Cauldron", LogLevel.Critical, DateTime.Now);
        public static void LogError(string message) => Log(message, "Cauldron", LogLevel.Error, DateTime.Now);
        public static void LogWarning(string message) => Log(message, "Cauldron", LogLevel.Warning, DateTime.Now);
        public static void LogInfo(string message) => Log(message, "Cauldron", LogLevel.Info, DateTime.Now);
        public static void LogDebug(string message) => Log(message, "Cauldron", LogLevel.Debug, DateTime.Now);

        public static void LogCritical(string source, string message) => Log(message, source, LogLevel.Critical, DateTime.Now);
        public static void LogError(string source, string message) => Log(message, source, LogLevel.Error, DateTime.Now);
        public static void LogWarning(string source, string message) => Log(message, source, LogLevel.Warning, DateTime.Now);
        public static void LogInfo(string source, string message) => Log(message, source, LogLevel.Info, DateTime.Now);
        public static void LogDebug(string source, string message) => Log(message, source, LogLevel.Debug, DateTime.Now);
        
        public static void LogCritical(string source, string message, DateTime dateTime) => Log(message, source, LogLevel.Critical, dateTime);
        public static void LogError(string source, string message, DateTime dateTime) => Log(message, source, LogLevel.Error, dateTime);
        public static void LogWarning(string source, string message, DateTime dateTime) => Log(message, source, LogLevel.Warning, dateTime);
        public static void LogInfo(string source, string message, DateTime dateTime) => Log(message, source, LogLevel.Info, dateTime);
        public static void LogDebug(string source, string message, DateTime dateTime) => Log(message, source, LogLevel.Debug, dateTime);

        private static void Log(string message, string source, LogLevel logLevel, DateTime dateTime)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _logEntries.Add(new LogEntry(message, source, logLevel, dateTime));

                Console.WriteLine($"[{Enum.GetName(typeof(LogLevel), logLevel)}][{source}] {message}");
                filteredLogEntries.Refresh();
            }));
        }


        private static bool FilterLogEntries(LogEntry logEntry, LogLevel logLevelMask, params string[] sources)
        {
            return sources.Contains(logEntry.Source) && (logLevelMask & logEntry.Level) != LogLevel.None;
        }
    }

    public class LogEntry
    {
        public string Message { get; }
        public string Source { get; }
        public LogLevel Level { get; }
        public DateTime Time { get; }

        public LogEntry(string message, string source, LogLevel level, DateTime time)
        {
            Message = message;
            Source = source;
            Level = level;
            Time = time;
        }

        public override string ToString()
        {
            return $"[{Time.ToString("G", CultureInfo.CurrentCulture)}][{Source}][{Level}] {Message}";
        }
    }

    [Flags]
    public enum LogLevel
    {
        None = 0,
        Critical = 1,
        Error = 2,
        Warning = 4,
        Info = 8,
        Debug = 16,
    }
}
