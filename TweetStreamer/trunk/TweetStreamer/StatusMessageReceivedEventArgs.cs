using System;

namespace TweetStreamer
{
    /// <summary>
    /// TweetStreamer
    /// </summary>
    internal class StatusMessageReceivedEventArgs : EventArgs, IStatusMessageReceivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusMessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public StatusMessageReceivedEventArgs(IStatusMessage message)
        {
            this.Message = message;
        }

        #region IOnStatusMessageReceivedEventArgs Members

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public IStatusMessage Message
        {
            get;
            set;
        }

        #endregion
    }
}
