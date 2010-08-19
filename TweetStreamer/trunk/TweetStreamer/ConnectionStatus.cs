using System;

namespace TweetStreamer
{
    /// <summary>
    /// Represents the connection status to the data stream.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// No connection has been established.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Attempting to connect.
        /// </summary>
        Connecting,

        /// <summary>
        /// Connected to the data stream.
        /// </summary>
        Connected,

        /// <summary>
        /// Attempting to disconnect.
        /// </summary>
        Disconnecting
    }
}
