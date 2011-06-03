using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KwwikaTweetStreamerPublisher
{
    public class LogOnlyPublisher: IMessagePublisher
    {
        private LoggerWrapper logger;

        public LogOnlyPublisher(LoggerWrapper logger)
        {
            this.logger = logger;
        }
        #region IMessagePublisher Members

        public void Disconnect()
        {
            logger.LogInfo("Disconnect called");
        }

        public void Publish(string topic, Dictionary<string, string> publishParams)
        {
            logger.LogInfo("Publish called for topic: " + topic);
        }

        #endregion
    }
}
