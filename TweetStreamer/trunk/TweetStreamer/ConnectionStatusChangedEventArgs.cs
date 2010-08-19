using System;

namespace TweetStreamer
{
    /// <summary>
    /// TweetStreamer
    /// </summary>
    internal class ConnectionStatusChangedEventArgs: IConnectionStatusChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        public ConnectionStatusChangedEventArgs(ConnectionStatus status)
        {
            this.Status = status;
        }

        #region IConnectionStatusChangedEventArgs Members

        /// <summary>
        /// Gets or sets the data stream connection status.
        /// </summary>
        /// <value>The status.</value>
        public ConnectionStatus Status
        {
            get;
            set;
        }

        #endregion
    }
}
