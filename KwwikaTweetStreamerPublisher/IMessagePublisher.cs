using System;
using System.Collections.Generic;
namespace KwwikaTweetStreamerPublisher
{
    interface IMessagePublisher
    {
        void Disconnect();
        void Publish(string topic, Dictionary<string, string> publishParams);
    }
}
