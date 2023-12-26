using System;
using System.Runtime.CompilerServices;

namespace PlayHouse.Utils
{
    public interface IPlayHouseLogger
    {
        void Debug(Func<string> messageFactory, string className,string methodName);
        void Info(Func<string> messageFactory, string className,string methodName);
        void Warn(Func<string> messageFactory, string className,string methodName);
        void Error(Func<string> messageFactory, string className,string methodName);
        void Trace(Func<string> messageFactory, string className,string methodName);
        void Fatal(Func<string> messageFactory, string className,string methodName);
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

        public void Trace(Func<string> messageFactory, string className,string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} TRACE: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Debug(Func<string> messageFactory, string className,string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} DEBUG: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Info(Func<string> messageFactory, string className,string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} INFO: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Warn(Func<string> messageFactory, string className,string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} WARN: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Error(Func<string> messageFactory, string className,string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} ERROR: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Fatal(Func<string> messageFactory, string className,string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} FATAL: [{className}] ({methodName}) {messageFactory()}");
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
        public static void  Trace(Func<string> messageFactory,string className,string methodName)
        {
            if (LogLevel.Trace >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Trace(messageFactory, className,methodName);
            }
        }

        public static void Debug(Func<string> messageFactory,string className,string methodName)
        {
            if (LogLevel.Debug >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Debug(messageFactory, className, methodName);
            }
        }

        public static void Info(Func<string> messageFactory,string className,string methodName)
        {
            if (LogLevel.Info >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Info(messageFactory, className, methodName);
            }
        }

        public static void Warn(Func<string> messageFactory,string className,string methodName)
        {
            if (LogLevel.Warning >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Warn(messageFactory, className,methodName);
            }
        }

        public static void Error(Func<string> messageFactory,string className,string methodName)
        {
            if (LogLevel.Error >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Error(messageFactory, className,methodName);
            }
        }
    
        public  static void Fatal(Func<string> messageFactory,string className,string methodName)
        {
            if (LogLevel.Fatal >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Fatal(messageFactory, className,methodName);
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

        public void  Trace(Func<string> messageFactory,[CallerMemberName] string methodName = "")
        {
            if (LogLevel.Trace >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Trace(messageFactory, _typeName,methodName);
            }
        }

        public void Debug(Func<string> messageFactory,[CallerMemberName] string methodName = "")
        {
            if (LogLevel.Debug >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Debug(messageFactory, _typeName,methodName);
            }
        }

        public void Info(Func<string> messageFactory,[CallerMemberName] string methodName = "")
        {
            if (LogLevel.Info >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Info(messageFactory, _typeName,methodName);
            }
        }

        public void Warn(Func<string> messageFactory,[CallerMemberName] string methodName = "")
        {
            if (LogLevel.Warning >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Warn(messageFactory, _typeName,methodName);
            }
        }

        public void Error(Func<string> messageFactory,[CallerMemberName] string methodName = "")
        {
            if (LogLevel.Error >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Error(messageFactory, _typeName,methodName);
            }
        }
        
        public  void Fatal(Func<string> messageFactory,[CallerMemberName] string methodName = "")
        {
            if (LogLevel.Fatal >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Fatal(messageFactory, _typeName,methodName);
            }
        }
    }
}
