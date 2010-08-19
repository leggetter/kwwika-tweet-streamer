using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;

namespace TweetStreamer
{
    /*
    "in_reply_to_status_id":null,
    "in_reply_to_user_id":18900335,
    "favorited":false,
    "in_reply_to_screen_name":"liviasalves",
    "text":"@liviasalves me p\u00f5e no #FF e voc\u00ea pr\u00f3pria n\u00e3o me segue.. n\u00e3 n\u00e3 \/\/magoei",
    "id":2820354600,
    "created_at":"Fri Jul 24 15:39:33 +0000 2009",
    "truncated":false,
    "source":"<a href=\"http:\/\/twitterfox.net\/\">TwitterFox<\/a>"
     
     
     {"favorited":false,
     "text":"An anonymous donor just became #3 to contribute to SFSA's #WorldCup Project. Thank you! Want to be #4? http://ht.ly/1YBU7",
     "contributors":null,
     "coordinates":null,
     "in_reply_to_user_id":null,
     "source":"<a href=\"http://www.hootsuite.com\" rel=\"nofollow\">HootSuite</a>",
     "geo":null,
     "created_at":"Tue Jun 15 08:59:03 +0000 2010",
     "in_reply_to_screen_name":null,
     "place":null,
     "user":
          {"time_zone":"Eastern Time (US & Canada)",
          "screen_name":"carolinecollie",
          "profile_link_color":"68563b",
          "url":"http://www.carolinecollie..com/",
          "profile_background_image_url":"http://a3.twimg.com/profile_background_images/33569889/nvzebrafur_twitter.br.jpg",
          "statuses_count":300,
          "profile_sidebar_fill_color":"b3a48f",
          "description":"A would be talk-show host&lover of polka-dots and bacon. A former party-girl & present day Mom. A missionary in South Africa w/ many-a-thought about life.",
          "profile_background_tile":false,
          "contributors_enabled":false,
          "profile_sidebar_border_color":"4c3e29",
          "lang":"en",
          "geo_enabled":false,
          "notifications":null,
          "created_at":"Wed Aug 19 01:04:26 +0000 2009",
          "followers_count":121,
          "following":null,
          "friends_count":152,
          "protected":false,
          "verified":false,
          "profile_background_color":"000000",
          "location":"South Africa",
          "name":"Caroline Collie",
          "favourites_count":0,
          "profile_image_url":"http://a3.twimg.com/profile_images/982603115/1382bc6d-c3cd-47b9-907b-7a095198dd6f_normal.png",
          "id":66862470,"utc_offset":-18000,
          "profile_text_color":"000000"},
     "truncated":false,
     * "id":16211141695,
     * "in_reply_to_status_id":null}

    */

    [DataContract]
    public class StatusMessage : IStatusMessage, INotifyPropertyChanged
    {
        private const char MESSAGE_END_CHARACTER = '\r';

        private static DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(StatusMessage));

        private TwitterUser _user;
        private DateTime _createdAt;

        private static ILogger _logger = new ConsoleLogger();
        public static ILogger Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        public StatusMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusMessage"/> class.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        public StatusMessage(string messageText)
        {
            this.Text = messageText;
        }

        #region IStatusMessage Members

        private string _Id;
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        [DataMember(Name = "id")]
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private string _InReplyToStatusId;
        /// <summary>
        /// Gets the in reply to status id.
        /// </summary>
        /// <value>The in reply to status id.</value>
        [DataMember(Name = "in_reply_to_status_id")]
        public string InReplyToStatusId
        {
            get
            {
                return _InReplyToStatusId;
            }
            set
            {
                _InReplyToStatusId = value;
                NotifyPropertyChanged("InReplyToStatusId");
            }
        }

        private string _InReplyToUserId;
        /// <summary>
        /// Gets the in reply to user id.
        /// </summary>
        /// <value>The in reply to user id.</value>
        [DataMember(Name = "in_reply_to_user_id")]
        public string InReplyToUserId
        {
            get
            {
                return _InReplyToUserId;
            }
            set
            {
                _InReplyToUserId = value;
                NotifyPropertyChanged("InReplyToUserId");
            }
        }

        private string _InReplyToScreenName;
        /// <summary>
        /// Gets the name of the in reply to screen.
        /// </summary>
        /// <value>The name of the in reply to screen.</value>
        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName
        {
            get
            {
                return _InReplyToScreenName;
            }
            set
            {
                _InReplyToScreenName = value;
                NotifyPropertyChanged("InReplyToScreenName");
            }
        }

        /// <summary>
        /// Gets the DateTime the message was created.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt
        {
            get
            {
                return this._createdAt;
            }
        }

        /// <summary>
        /// Gets or sets DateTime the message was created at in String format.
        /// </summary>
        /// <value>The created at string.</value>
        [DataMember(Name = "created_at")]
        public string CreatedAtString
        {
            get
            {
                return this._createdAt.ToString();
            }
            set
            {
                this._createdAt = ParseDateTime(value);
                NotifyPropertyChanged("CreatedAtString");
            }
        }

        private bool _Truncated;
        /// <summary>
        /// Gets a value indicating whether this <see cref="IStatusMessage"/> is truncated.
        /// </summary>
        /// <value><c>true</c> if truncated; otherwise, <c>false</c>.</value>
        [DataMember(Name = "truncated")]
        public bool Truncated
        {
            get
            {
                return _Truncated;
            }
            set
            {
                _Truncated = value;
                NotifyPropertyChanged("Truncated");
            }
        }

        private bool _Favourited;
        /// <summary>
        /// Gets a value indicating whether this <see cref="IStatusMessage"/> is favourited.
        /// </summary>
        /// <value><c>true</c> if favourited; otherwise, <c>false</c>.</value>
        [DataMember(Name = "favourited")]
        public bool Favourited
        {
            get
            {
                return _Favourited;
            }
            set
            {
                _Favourited = value;
                NotifyPropertyChanged("Favourited");
            }
        }

        private string _Source;
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        [DataMember(Name = "source")]
        public string Source
        {
            get
            {
                return _Source;
            }
            set
            {
                _Source = value;
                NotifyPropertyChanged("Source");
            }
        }

        private string _Text;
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        [DataMember(Name = "text")]
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                NotifyPropertyChanged("Text");
            }
        }

        private Geo _geo;
        [DataMember(Name = "geo")]
        public Geo Geo
        {
            get
            {
                return _geo;
            }
            set
            {
                _geo = value;
                NotifyPropertyChanged("Geo");
            }
        }

        private Place _place;
        /// <summary>
        /// Gets the place information.
        /// </summary>
        /// <value>The text.</value>
        [DataMember(Name = "place")]
        public Place Place
        {
            get
            {
                return _place;
            }
            set
            {
                _place = value;
                NotifyPropertyChanged("Place");
            }
        }

        /// <summary>
        /// Gets the Twitter user.
        /// </summary>
        /// <value>The user.</value>
        [DataMember(Name="user")]
        public TwitterUser User
        {
            get
            {
                return this._user;
            }
            set
            {
                this._user = value;
                NotifyPropertyChanged("User");
            }
        }

        /// <summary>
        /// Gets the Twitter user.
        /// </summary>
        /// <value>The user.</value>
        ITwitterUser IStatusMessage.User
        {
            get
            {
                return this._user;
            }
        }

        #endregion

        /// <summary>
        /// Parses a Twitter CreatedAt and returns a DateTime.
        /// </summary>
        /// <param name="date">The Twitter date.</param>
        /// <remarks>The Twitter date format is as follows: </remarks>
        /// <returns>The corresponding DateTime.</returns>
        private static DateTime ParseDateTime(string date)
        {
            var builder = new StringBuilder(date.Length + 1);

            DateTime parsedDate;

            if (DateTime.TryParse(date, out parsedDate) == false)
            {
                builder.Append(date.Substring(0, 23)).Append(':').Append(date.Substring(23));
                parsedDate = DateTime.ParseExact(builder.ToString(), "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
            }
            return parsedDate;
        }

        /// <summary>
        /// Creates a number of Twitter messages from json string.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        internal static IStatusMessage[] CreateMessagesFromJsonString(string json)
        {
            IList<IStatusMessage> messages = new List<IStatusMessage>();

            string[] messageStrings = json.Split(MESSAGE_END_CHARACTER);
            foreach (string messageData in messageStrings)
            {
                string jsonMessageData = messageData.Trim();
                if (jsonMessageData.Length > 0)
                {
                    try
                    {
                        IStatusMessage message = CreateMessageFromJsonString(jsonMessageData);
                        if (message != null)
                        {
                            messages.Add(message);
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                }
            }
            return messages.ToArray<IStatusMessage>();
        }

        /// <summary>
        /// Creates a single message from json string.
        /// </summary>
        /// <param name="messageData">The message data.</param>
        /// <returns></returns>
        internal static IStatusMessage CreateMessageFromJsonString(string messageData)
        {
            _logger.LogInfo(String.Format("Creating StatusMessage for: {0}", messageData));

            IStatusMessage message = null;

            using (MemoryStream stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(messageData)))
            {
                message = ser.ReadObject(stream) as StatusMessage;
            }

            if (string.IsNullOrEmpty(message.Id))
            {
                message = null;
                _logger.LogInfo("message had no ID. Assuming to be a delete message so nulling message object");
            }

            return message;
        }

        /// <summary>
        /// Helper method to check if a string contains a full message.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <returns></returns>
        internal static bool StringContainsFullMessage(string stringToCheck)
        {
            return stringToCheck.Contains(MESSAGE_END_CHARACTER);
        }

        /// <summary>
        /// Checks for excess data and returns any data that will not be used to create a status message.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The excess data not used in creating a status message; a partial status message</returns>
        internal static string CheckForExcessData(string data)
        {
            string leftOverData = string.Empty;
            int lastCarriageReturnIndex = data.LastIndexOf(MESSAGE_END_CHARACTER);

            // only substring if there is data after the last message end character
            if((data.Length-1) > lastCarriageReturnIndex)
            {
                leftOverData = data.Substring(lastCarriageReturnIndex+1);
            }

            return leftOverData;
        }

        /// <summary>
        /// Helper method to facilitate the notification of any observers that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
