using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KwwikaTweetStreamerPublisher
{
    public class MessagePublisherProxy: IMessagePublisher
    {
        List<IMessagePublisher> _publishers = new List<IMessagePublisher>();

        public MessagePublisherProxy(LoggerWrapper logger, KwwikaTweetStreamerPublisherConfig config)
        {
            _publishers.Add(new LogOnlyPublisher(logger));
            _publishers.Add(new PusherPublisher(logger));
            _publishers.Add(new KwwikaPublisher(logger, config.KwwikaConfig.ApiKey, config.KwwikaConfig.Domain));
        }
        #region IMessagePublisher Members

        public void Disconnect()
        {
            foreach (var publisher in _publishers)
            {
                publisher.Disconnect();
            }
        }

        public void Publish(string topic, Dictionary<string, string> publishParams)
        {
            foreach (var publisher in _publishers)
            {
                publisher.Publish(topic, publishParams);
            }
        }
        #endregion
    }
}
