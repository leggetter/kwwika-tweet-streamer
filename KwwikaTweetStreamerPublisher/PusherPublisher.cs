using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PusherDotNet;
using System.Threading;

namespace KwwikaTweetStreamerPublisher
{
    public class PusherPublisher : IMessagePublisher
    {
        private LoggerWrapper _logger;
        private IPusherProvider _provider;

        public PusherPublisher(LoggerWrapper _logger)
        {
            this._logger = _logger;
            _provider = new PusherProvider("3016", "1db0d58e74e26c2dc0b2", "e1412346f15106d44ce1");
        }

        public void Publish(string topic, Dictionary<string, string> publishParams)
        {
            string channelName = topicToChannelName(topic);
            string eventName = "tweet";
            //string jsonData = publishParamsToJsonData(publishParams);
            //SimplePusherRequest request = new SimplePusherRequest(channelName, eventName, jsonData);
            object data = publishParamsToObject(publishParams);
            ObjectPusherRequest request = new ObjectPusherRequest(channelName, eventName, data);


            ThreadStart ts = new ThreadStart(delegate()
                {
                    try
                    {
                        _provider.Trigger(request);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("error publishing to Pusher", ex);
                    }
                });

            Thread thread = new Thread(ts);
            thread.Start();
        }

        private object publishParamsToObject(Dictionary<string, string> publishParams)
        {
            return new { ScreenName = publishParams["ScreenName"], Text = publishParams["Text"] };
        }

        private string topicToChannelName(string topic)
        {
            return topic.Replace('/', '_').Substring(1, topic.Length - 1);
        }

        private string publishParamsToJsonData(Dictionary<string, string> publishParams)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (string name in publishParams.Keys)
            {
                sb.AppendLine("\"" + name + "\":\"" + publishParams[name] + "\"");
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1); // remove last comma
            sb.AppendLine("}");

            return sb.ToString();
        }

        public void Disconnect()
        {
            // do nothing
        }
    }
}
