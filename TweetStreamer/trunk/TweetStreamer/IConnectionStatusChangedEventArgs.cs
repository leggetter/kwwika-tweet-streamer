using System;

namespace TweetStreamer
{
    public interface IConnectionStatusChangedEventArgs
    {
        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <value>The status.</value>
        ConnectionStatus Status
        {
            get;
        }
    }
}
