using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetStreamer;
using Kwwika.Common.Logging;
using Kwwika.Common.Logging.NLog;

namespace KwwikaTweetStreamerPublisher
{
    public class LoggerWrapper : ILogger, Kwwika.Logging.ILogger
    {
        private ILoggingService _loggingService = new LoggingService("KwwikaTweetStreamerPublisher");
        private ILogger _internalLogger = new ConsoleLogger();
        public LoggerWrapper()
        {
        }

        #region ILogger Members

        public void LogInfo(string message)
        {
            _internalLogger.LogInfo(message);
            _loggingService.Info(message);
        }

        public void LogWarn(string message)
        {
            _internalLogger.LogInfo(message);
            _loggingService.Warn(message);
        }

        public void LogError(string message)
        {
            _internalLogger.LogError(message);
            _loggingService.Error(message);
        }

        public void LogError(Exception ex)
        {
            LogError(ex.ToString());
        }

        public void LogError(string message, Exception ex)
        {
            LogError(message + " " + ex.ToString());
        }

        #endregion

        #region ILogger Members

        public void Log(Kwwika.Logging.LogLevels level, string[] categories, string message, params string[] messageParameters)
        {
            Log(level, categories, string.Format(message, messageParameters));
        }

        public void Log(Kwwika.Logging.LogLevels level, string[] categories, string message)
        {
            message = string.Join(",", categories) + " " + message;
            if (level == Kwwika.Logging.LogLevels.Error || level == Kwwika.Logging.LogLevels.Critical)
            {
                LogError(message);
            }
            else if (level == Kwwika.Logging.LogLevels.Warning)
            {
                LogWarn(message);
            }
            else
            {
                LogInfo(message);
            }
        }

        public void Log(Kwwika.Logging.LogLevels level, string category, string message, params string[] messageParameters)
        {
            Log(level, new string[] { category }, string.Format(message, messageParameters));
        }

        public void Log(Kwwika.Logging.LogLevels level, string category, string message)
        {
            Log(level, new string[] { category }, message);
        }

        public void Log(Kwwika.Logging.LogLevels level, string[] categories, string message, params object[] messageParameters)
        {
            Log(level, categories, string.Format(message, messageParameters));
        }

        public void Log(Kwwika.Logging.LogLevels level, string category, string message, params object[] messageParameters)
        {
            Log(level, new string[]{category}, message, messageParameters);
        }

        #endregion
    }
}
