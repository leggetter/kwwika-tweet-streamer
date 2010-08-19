using System;

namespace TweetStreamer
{
    /// <summary>
    /// TweetStreamer
    /// </summary>
    public interface IStatusMessageReceivedEventArgs
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        IStatusMessage Message
        {
            get;
        }
    }
}
