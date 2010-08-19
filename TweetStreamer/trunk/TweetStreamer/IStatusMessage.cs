using System;

namespace TweetStreamer
{
    /// <summary>
    /// ReTweetStreamerter message (Tweet) from the Twitter data stream.
    /// </summary>
    public interface IStatusMessage
    {
        /// <summary>
        /// Gets the in reply to status id.
        /// </summary>
        /// <value>The in reply to status id.</value>
        string InReplyToStatusId
        {
            get;
        }

        /// <summary>
        /// Gets the in reply to user id.
        /// </summary>
        /// <value>The in reply to user id.</value>
        string InReplyToUserId
        {
            get;
        }

        /// <summary>
        /// Gets the name of the in reply to screen.
        /// </summary>
        /// <value>The name of the in reply to screen.</value>
        string InReplyToScreenName
        {
            get;
        }

        /// <summary>
        /// Gets the created at.
        /// </summary>
        /// <value>The created at.</value>
        DateTime CreatedAt
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IStatusMessage"/> is truncated.
        /// </summary>
        /// <value><c>true</c> if truncated; otherwise, <c>false</c>.</value>
        bool Truncated
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IStatusMessage"/> is favourited.
        /// </summary>
        /// <value><c>true</c> if favourited; otherwise, <c>false</c>.</value>
        bool Favourited
        {
            get;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        string Source
        {
            get;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text
        {
            get;
        }


        /// <summary>
        /// Place information.
        /// </summary>
        Place Place
        {
            get;
        }

        /// <summary>
        /// Geolocation information.
        /// </summary>
        Geo Geo
        {
            get;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        string Id
        {
            get;
        }

        /// <summary>
        /// Gets the Twitter user.
        /// </summary>
        /// <value>The user.</value>
        ITwitterUser User
        {
            get;
        }
    }
}
