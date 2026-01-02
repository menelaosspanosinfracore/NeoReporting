using System;
using NLog;

namespace NeoReporting.Logging
{
    public class NLogLogger : ILoggerService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Warn(string message)
        {
            Logger.Warn(message);
        }

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Error(string message, Exception ex = null)
        {
            if (ex == null)
                Logger.Error(message);
            else
                Logger.Error(ex, message);
        }
    }
}
