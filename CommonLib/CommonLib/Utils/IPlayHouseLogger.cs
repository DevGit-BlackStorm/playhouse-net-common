using System;

namespace PlayHouse.Utils
{
    public interface IPlayHouseLogger
    {
        void Debug(Func<string> messageFactory, string className);
        void Info(Func<string> messageFactory, string className);
        void Warn(Func<string> messageFactory, string className);
        void Error(Func<string> messageFactory, string className);
        void Trace(Func<string> messageFactory, string className);
        void Fatal(Func<string> messageFactory, string className);
    }

    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }

    public class ConsoleLogger : IPlayHouseLogger
    {
        private string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public void Trace(Func<string> messageFactory, string className)
        {
            Console.WriteLine($"{GetTimeStamp()} TRACE: [{className}] - {messageFactory()}");
        }

        public void Debug(Func<string> messageFactory, string className)
        {
            Console.WriteLine($"{GetTimeStamp()} DEBUG: [{className}] - {messageFactory()}");
        }

        public void Info(Func<string> messageFactory, string className)
        {
            Console.WriteLine($"{GetTimeStamp()} INFO: [{className}] - {messageFactory()}");
        }

        public void Warn(Func<string> messageFactory, string className)
        {
            Console.WriteLine($"{GetTimeStamp()} WARN: [{className}] - {messageFactory()}");
        }

        public void Error(Func<string> messageFactory, string className)
        {
            Console.WriteLine($"{GetTimeStamp()} ERROR: [{className}] - {messageFactory()}");
        }

        public void Fatal(Func<string> messageFactory, string className)
        {
            Console.WriteLine($"{GetTimeStamp()} FATAL: [{className}] - {messageFactory()}");
        }
    }

    public static class LoggerConfigure
    {
        private static IPlayHouseLogger _logger = new ConsoleLogger();
        private static LogLevel _logLevel = LogLevel.Debug;
        public static void SetLogger(IPlayHouseLogger logger , LogLevel logLevel )
        {
            _logger = logger;
            _logLevel = logLevel;
        }

        public static IPlayHouseLogger Log => _logger;
        public static LogLevel LogLevel => _logLevel;
    }

    public static class StaticLOG
    {
        public static void  Trace(Func<string> messageFactory,string className)
        {
            if (LogLevel.Trace >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Trace(messageFactory, className);
            }
        }

        public static void Debug(Func<string> messageFactory,string className)
        {
            if (LogLevel.Debug >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Debug(messageFactory, className);
            }
        }

        public static void Info(Func<string> messageFactory,string className)
        {
            if (LogLevel.Info >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Info(messageFactory, className);
            }
        }

        public static void Warn(Func<string> messageFactory,string className)
        {
            if (LogLevel.Warning >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Warn(messageFactory, className);
            }
        }

        public static void Error(Func<string> messageFactory,string className)
        {
            if (LogLevel.Error >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Error(messageFactory, className);
            }
        }
    
        public  static void Fatal(Func<string> messageFactory,string className)
        {
            if (LogLevel.Fatal >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Fatal(messageFactory, className);
            }
        }
      
    }
    public  class LOG<T>
    {
        
        private readonly string _typeName;

        public LOG()
        {
            _typeName = typeof(T).FullName ?? typeof(T).Name;
        }
        
        public void  Trace(Func<string> messageFactory)
        {
            if (LogLevel.Trace >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Trace(messageFactory, _typeName);
            }
        }

        public void Debug(Func<string> messageFactory)
        {
            if (LogLevel.Debug >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Debug(messageFactory, _typeName);
            }
        }

        public void Info(Func<string> messageFactory)
        {
            if (LogLevel.Info >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Info(messageFactory, _typeName);
            }
        }

        public void Warn(Func<string> messageFactory)
        {
            if (LogLevel.Warning >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Warn(messageFactory, _typeName);
            }
        }

        public void Error(Func<string> messageFactory)
        {
            if (LogLevel.Error >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Error(messageFactory, _typeName);
            }
        }

        
        public  void Fatal(Func<string> messageFactory)
        {
            if (LogLevel.Fatal >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Fatal(messageFactory, _typeName);
            }
        }
    }
}
