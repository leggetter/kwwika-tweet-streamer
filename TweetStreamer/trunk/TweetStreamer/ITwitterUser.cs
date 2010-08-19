using System;
namespace TweetStreamer
{
    /// <summary>
    /// Represents a Twitter user
    /// </summary>
    public interface ITwitterUser
    {
        /// <summary>
        /// Gets or sets the followers count.
        /// </summary>
        /// <value>The followers count.</value>
        int FollowersCount { get; set; }
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The id.</value>
        string Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the profile image URL.
        /// </summary>
        /// <value>The profile image URL.</value>
        string ProfileImageUrl { get; set; }
        /// <summary>
        /// Gets or sets the name of the screen.
        /// </summary>
        /// <value>The name of the screen.</value>
        string ScreenName { get; set; }
    }
}
