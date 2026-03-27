using System;
using System.Diagnostics;
using System.Text;

namespace ContosoUniversity.Logging
{
    public class Logger : ILogger
    {
        // -----------------------------
        // Information
        // -----------------------------
        public void Information(string message) =>
            Trace.TraceInformation(message);

        public void Information(string message, params object[] args) =>
            Trace.TraceInformation(message, args);

        public void Information(Exception exception, string message, params object[] args) =>
            Trace.TraceInformation(FormatExceptionMessage(exception, message, args));

        // -----------------------------
        // Warning
        // -----------------------------
        public void Warning(string message) =>
            Trace.TraceWarning(message);

        public void Warning(string message, params object[] args) =>
            Trace.TraceWarning(message, args);

        public void Warning(Exception exception, string message, params object[] args) =>
            Trace.TraceWarning(FormatExceptionMessage(exception, message, args));

        // -----------------------------
        // Error
        // -----------------------------
        public void Error(string message) =>
            Trace.TraceError(message);

        public void Error(string message, params object[] args) =>
            Trace.TraceError(message, args);

        public void Error(Exception exception, string message, params object[] args) =>
            Trace.TraceError(FormatExceptionMessage(exception, message, args));

        // -----------------------------
        // API Tracing
        // -----------------------------
        public void TraceApi(string component, string method, TimeSpan duration) =>
            TraceApi(component, method, duration, string.Empty);

        public void TraceApi(string component, string method, TimeSpan duration, string message, params object[] args) =>
            TraceApi(component, method, duration, string.Format(message, args));

        public void TraceApi(string component, string method, TimeSpan duration, string properties)
        {
            var msg = $"Component:{component}; Method:{method}; Duration:{duration}; Properties:{properties}";
            Trace.TraceInformation(msg);
        }

        // -----------------------------
        // Helpers
        // -----------------------------
        private static string FormatExceptionMessage(Exception exception, string message, params object[] args)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(message))
                sb.AppendFormat(message, args);

            sb.Append(" | Exception: ");
            sb.Append(exception);

            return sb.ToString();
        }

    }
}