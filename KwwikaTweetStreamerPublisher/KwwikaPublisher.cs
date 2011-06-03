using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kwwika;
using Kwwika.Logging;

namespace KwwikaTweetStreamerPublisher
{


    public class KwwikaPublisher : IConnectionListener, ICommandListener, IMessagePublisher
    {
        IConnection _kwwikaConnection;
        LoggerWrapper _logWrapper;

        public KwwikaPublisher(LoggerWrapper logger, string apiKey, string domain)
        {
            _kwwikaConnection = Kwwika.Service.Connect(apiKey, domain, this);
            _logWrapper = logger;
            _kwwikaConnection.Logger = logger;
        }

        #region IConnectionListener Members

        public void ConnectionStatusUpdated(Kwwika.ConnectionStatus status)
        {
            _connectionStatus = status;
        }

        #endregion

        #region ICommandListener Members

        public void CommandError(string topic, CommandErrorType code)
        {
            _logWrapper.LogError("failed to publish to" + topic);
        }

        public void CommandSuccess(string topic)
        {
            _logWrapper.LogInfo("successfully to publish to" + topic);
        }

        #endregion

        public void Publish(string topic, Dictionary<string, string> publishParams)
        {
            if (_connectionStatus == ConnectionStatus.LoggedIn ||
                _connectionStatus == ConnectionStatus.Reconnected)
            {
                _kwwikaConnection.Publish(topic, publishParams, this);
            }
            else
            {
                _logWrapper.LogWarn("Not connecting. Ignoring publish request.");
            }
        }

        public void Disconnect()
        {
            _kwwikaConnection.Disconnect();
        }

        public ConnectionStatus _connectionStatus { get; set; }
    }
}
