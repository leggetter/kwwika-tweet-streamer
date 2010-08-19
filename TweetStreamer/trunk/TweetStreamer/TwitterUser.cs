using System;
using System.Runtime.Serialization;

namespace TweetStreamer
{
    /*
    "user":
    {"profile_sidebar_border_color":"ffffff",
     "profile_background_tile":true,
     "utc_offset":-14400,
     "favourites_count":2,
     "url":null,
     "profile_image_url":"http:\/\/s3.amazonaws.com\/twitter_production\/profile_images\/318684558\/twitterProfilePhoto_normal.jpg",
     "time_zone":"Santiago",
     "profile_background_image_url":"http:\/\/s3.amazonaws.com\/twitter_production\/profile_background_images\/24548886\/abstrato.jpg",
     "profile_background_color":"000000",
     "screen_name":"mathsn",
     "notifications":null,
     "name":"Matheus ",
     "statuses_count":96,
     "profile_text_color":"000000",
     "protected":false,
     "location":"",
     "description":"",
     "profile_link_color":"cb0dff",
     "followers_count":42,
     "following":null,
     "verified":false,
     "created_at":"Wed Nov 05 19:00:46 +0000 2008",
     "profile_sidebar_fill_color":"f5d2f5",
     "id":17194966,
     "friends_count":28}
    */
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class TwitterUser : ITwitterUser
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DataMember(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name="name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the screen.
        /// </summary>
        /// <value>The name of the screen.</value>
        [DataMember(Name = "screen_name")]
        public string ScreenName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember(Name = "description")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [DataMember(Name = "location")]
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the statuses count.
        /// </summary>
        /// <value>The statuses count.</value>
        [DataMember(Name = "statuses_count")]
        public int StatusesCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the followers count.
        /// </summary>
        /// <value>The followers count.</value>
        [DataMember(Name = "followers_count")]
        public int FollowersCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the favourites count.
        /// </summary>
        /// <value>The favourites count.</value>
        [DataMember(Name = "favourites_count")]
        public int FavouritesCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the profile image URL.
        /// </summary>
        /// <value>The profile image URL.</value>
        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl
        {
            get;
            set;
        }
    }
}
