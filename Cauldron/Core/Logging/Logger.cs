using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cauldron.Core.Logging
{
    public static class Logger
    {
        private static ObservableCollection<LogEntry> logEntries = new ObservableCollection<LogEntry>();

        private static LogLevel _logLevelMask = LogLevel.Critical | LogLevel.Error | LogLevel.Warning | LogLevel.Info;
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
                var source = CollectionViewSource.GetDefaultView(logEntries);
                source.Filter = o => FilterLogEntries((LogEntry)o, _logLevelMask, _sourcesMask);
                return source;
            }
        }
        
        public static void LogCritical(string source, string message) => Log(message, source, LogLevel.Critical);
        public static void LogError(string source, string message) => Log(message, source, LogLevel.Error);
        public static void LogWarning(string source, string message) => Log(message, source, LogLevel.Warning);
        public static void LogInfo(string source, string message) => Log(message, source, LogLevel.Info);
        public static void LogDebug(string source, string message) => Log(message, source, LogLevel.Debug);

        private static void Log(string message, string source, LogLevel logLevel)
        {
            logEntries.Add(new LogEntry(message, source, logLevel));
            Console.WriteLine($"[{Enum.GetName(typeof(LogLevel), logLevel)}][{source}] {message}");
            filteredLogEntries.Refresh();
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

        public LogEntry(string message, string source, LogLevel level)
        {
            Message = message;
            Source = source;
            Level = level;
        }

        public override string ToString()
        {
            return $"[{Source}][{Level}] {Message}";
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
