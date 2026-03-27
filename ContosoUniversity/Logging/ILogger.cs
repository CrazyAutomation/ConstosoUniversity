using System;

namespace ContosoUniversity.Logging
{
    public interface ILogger
    {

        // Information
        void Information(string message);
        void Information(string message, params object[] args);
        void Information(Exception exception, string message, params object[] args);

        // Warning
        void Warning(string message);
        void Warning(string message, params object[] args);
        void Warning(Exception exception, string message, params object[] args);

        // Error
        void Error(string message);
        void Error(string message, params object[] args);
        void Error(Exception exception, string message, params object[] args);

        // API Tracing
        void TraceApi(string component, string method, TimeSpan duration);
        void TraceApi(string component, string method, TimeSpan duration, string properties);
        void TraceApi(string component, string method, TimeSpan duration, string message, params object[] args);

    }
}