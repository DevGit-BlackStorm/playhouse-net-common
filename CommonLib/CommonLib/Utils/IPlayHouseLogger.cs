using System;
using System.Runtime.CompilerServices;

namespace PlayHouse.Utils
{

    public interface IPlayHouseLogger
    {
        void Debug(Func<FormattableString> messageFactory, string className, string methodName);
        void Info(Func<FormattableString> messageFactory, string className, string methodName);
        void Warn(Func<FormattableString> messageFactory, string className, string methodName);
        void Error(Func<FormattableString> messageFactory, string className, string methodName);
        void Trace(Func<FormattableString> messageFactory, string className, string methodName);
        void Fatal(Func<FormattableString> messageFactory, string className, string methodName);
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
      public void Trace(Func<FormattableString> messageFactory, string className, string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} TRACE: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Debug(Func<FormattableString> messageFactory, string className, string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} DEBUG: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Info(Func<FormattableString> messageFactory, string className, string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} INFO: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Warn(Func<FormattableString> messageFactory, string className, string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} WARN: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Error(Func<FormattableString> messageFactory, string className, string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} ERROR: [{className}] ({methodName}) {messageFactory()}");
        }

        public void Fatal(Func<FormattableString> messageFactory, string className, string methodName)
        {
            Console.WriteLine($"{GetTimeStamp()} FATAL: [{className}] ({methodName}) {messageFactory()}");
        }


        private string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
    public static class LoggerConfigure
    {
        public static IPlayHouseLogger Log { get; private set; } = new ConsoleLogger();

        public static LogLevel LogLevel { get; private set; } = LogLevel.Debug;


        public static LogLevel FLogLevel { get; private set; } = LogLevel.Debug;

        public static void SetLogger(IPlayHouseLogger logger, LogLevel logLevel)
        {
            Log = logger;
            LogLevel = logLevel;
        }
    }
    
   public class LOG<T>
    {
        private readonly string _typeName = typeof(T).FullName ?? typeof(T).Name;

        public void Trace(Func<FormattableString> messageFactory, [CallerMemberName] string methodName = "")
        {
            if (LogLevel.Trace >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Trace(messageFactory, _typeName, methodName);
            }
        }

        public void Debug(Func<FormattableString> messageFactory, [CallerMemberName] string methodName = "")
        {
            if (LogLevel.Debug >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Debug(messageFactory, _typeName, methodName);
            }
        }

        public void Info(Func<FormattableString> messageFactory, [CallerMemberName] string methodName = "")
        {
            if (LogLevel.Info >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Info(messageFactory, _typeName, methodName);
            }
        }

        public void Warn(Func<FormattableString> messageFactory, [CallerMemberName] string methodName = "")
        {
            if (LogLevel.Warning >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Warn(messageFactory, _typeName, methodName);
            }
        }

        public void Error(Func<FormattableString> messageFactory, [CallerMemberName] string methodName = "")
        {
            if (LogLevel.Error >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Error(messageFactory, _typeName, methodName);
            }
        }

        public void Fatal(Func<FormattableString> messageFactory, [CallerMemberName] string methodName = "")
        {
            if (LogLevel.Fatal >= LoggerConfigure.LogLevel)
            {
                LoggerConfigure.Log.Fatal(messageFactory, _typeName, methodName);
            }
        }
    }
}