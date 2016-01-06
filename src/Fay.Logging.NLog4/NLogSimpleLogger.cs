using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Fay.Logging.NLog4
{
    public sealed class NLogSimpleLogger : DelegateLogger<string>
    {
        private readonly IDictionary<LogSeverity, LogLevel> _logSeverityToLogLevelMap = new Dictionary<LogSeverity, LogLevel>
        {
            {LogSeverity.Off, LogLevel.Off},
            {LogSeverity.Critical, LogLevel.Fatal},
            {LogSeverity.Error, LogLevel.Error},
            {LogSeverity.Warning, LogLevel.Warn},
            {LogSeverity.Information, LogLevel.Info},
            {LogSeverity.Verbose, LogLevel.Debug},
            {LogSeverity.All, LogLevel.Trace}, 
        };

        private ILogger MyLogger { get; set; }
        public NLogSimpleLogger(ILogger logger)
        {
            Contract.Requires<ArgumentNullException>(logger != null);
            Contract.Ensures(MyLogger != null);

            MyLogger = logger;
        }

        public override bool IsSeverityInScope(LogSeverity severity, Func<string> messageDelegate)
        {
            return MyLogger.IsEnabled(_logSeverityToLogLevelMap[severity]);
        }

        protected override void Write(LogSeverity severity, Func<string> messageDelegate)
        {
            Write(severity, messageDelegate, null);
        }

        private void Write(LogSeverity severity, Func<string> messageDelegate, Exception ex)
        {
            if (!IsSeverityInScope(severity, null))
                return;

            string message = messageDelegate?.Invoke();

            if (message == null)
                return;

            MyLogger.Log(_logSeverityToLogLevelMap[severity], ex, message);
        }

        public override void Exception(LogSeverity severity, Func<string> messageDelegate, Exception ex)
        {
            if (messageDelegate == null && ex == null)
                return;

            Write(severity, messageDelegate, ex);
        }

        /// <summary>
        /// Method returns null as not required for NLog implementation
        /// </summary>
        protected override Func<string> InjectExceptionIntoMessageDelegate(Func<string> messageDelegate, Exception ex)
        {
            return null;
        }        
    }
}
