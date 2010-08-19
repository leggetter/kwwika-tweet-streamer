using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;
using System.Text.RegularExpressions;
using TweetStreamer;

namespace KwwikaTweetStreamerPublisher
{
    public class KwwikaTweetStreamerPublisherConfig
    {
        public TwitterConfig TwitterConfig
        {
            get;
            set;
        }

        public KwwikaConfig KwwikaConfig
        {
            get;
            set;
        }

        public SearchDefinition[] SearchDefinitions
        {
            get;
            set;
        }

        public static void CheckConfiguration(KwwikaTweetStreamerPublisherConfig config)
        {
            if (config.KwwikaConfig == null)
            {
                throw new ConfigurationErrorsException("KwwikaConfig must be defined");
            }
            if (string.IsNullOrEmpty(config.KwwikaConfig.Domain) ||
                string.IsNullOrEmpty(config.KwwikaConfig.ApiKey))
            {
                throw new ConfigurationErrorsException("KwwikaConfig.Username and KwwikaConfig.Password must all be defined");
            }
            if (config.TwitterConfig == null)
            {
                throw new ConfigurationErrorsException("TwitterConfig must be defined");
            }
            if (string.IsNullOrEmpty(config.TwitterConfig.Password) ||
                string.IsNullOrEmpty(config.TwitterConfig.Username) ||
                string.IsNullOrEmpty(config.TwitterConfig.Url))
            {
                throw new ConfigurationErrorsException("TwitterConfig.Username, TwitterConfig.Password and TwitterConfig.Url must all be defined");
            }
            if (config.SearchDefinitions == null || config.SearchDefinitions.Length == 0)
            {
                throw new ConfigurationErrorsException("SearchDefinitions must be defined and at least one SearchDefinition must be defined");
            }
            foreach (SearchDefinition def in config.SearchDefinitions)
            {
                if (string.IsNullOrEmpty(def.PublishTo))
                {
                    throw new ConfigurationErrorsException("SearchDefinition.PublishTo and SearchDefinition.TrackFor must all be defined");
                }
                if(string.IsNullOrEmpty(def.TrackFor) &&  string.IsNullOrEmpty(def.FollowUsersWithId))
                {
                    throw new ConfigurationErrorsException("At least one of SearchDefinition.TrackFor or SearchDefinition.FollowUsersWithId must all be defined");
                }
            }
        }
    }

    public class SearchDefinition
    {
        private bool _request = true;

        [XmlAttribute(DataType="string", AttributeName="Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(DataType = "boolean", AttributeName = "Request")]
        public bool Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
            }
        }

        public string TrackFor
        {
            get;
            set;
        }

        [XmlArray()]
        [XmlArrayItem(typeof(MatchExpression), ElementName="Match")]
        public MatchExpression[] Ignore
        {
            get;
            set;
        }

        public string FollowUsersWithId
        {
            get;
            set;
        }

        public string PublishTo
        {
            get;
            set;
        }

        public string FieldsToSend
        {
            get;
            set;
        }

        public CountField[] CountFields
        {
            get;
            set;
        }

        internal bool ShouldIgnoreTweet(IDictionary<string, string> tweetUpdate)
        {
            bool ignore = false;
            if (this.Ignore != null)
            {
                foreach (MatchExpression exp in this.Ignore)
                {
                    RegexOptions options = (exp.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                    Regex regex = new Regex(exp.Expression, options);
                    string fieldToMatch = tweetUpdate[exp.FieldToMatch];
                    if (regex.IsMatch(fieldToMatch) == true)
                    {
                        ignore = true;
                        break;
                    }
                }
            }
            return ignore;
        }

        internal bool IsTrackingFor(IStatusMessage statusMessage)
        {
            if (this.TrackFor != null)
            {
                foreach (string searchParam in this.TrackFor.Split(','))
                {
                    if (statusMessage.Text.ToUpper().Contains(searchParam.ToUpper()) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool IsFollowingUser(ITwitterUser twitterUser)
        {
            if (this.FollowUsersWithId != null)
            {
                foreach (string userId in this.FollowUsersWithId.Split(','))
                {
                    if (twitterUser != null &&
                        twitterUser.Id == userId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class TwitterConfig
    {
        private string _url = "http://stream.twitter.com/1/statuses/filter.json";
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

    }

    public class KwwikaConfig
    {
        public string Domain
        {
            get;
            set;
        }

        public string ApiKey
        {
            get;
            set;
        }
    }

    public class CountField
    {
        [XmlAttribute(DataType = "string", AttributeName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(DataType = "boolean", AttributeName = "MatchAll")]
        public bool MatchAll
        {
            get;
            set;
        }

        public MatchExpression Match
        {
            get;
            set;
        }
    }

    public class MatchExpression
    {
        [XmlAttribute(DataType = "boolean", AttributeName = "IgnoreCase")]
        public bool IgnoreCase
        {
            get;
            set;
        }

        [XmlAttribute(DataType = "string", AttributeName = "Expression")]
        public string Expression
        {
            get;
            set;
        }

        [XmlAttribute(DataType = "string", AttributeName = "FieldToMatch")]
        public string FieldToMatch
        {
            get;
            set;
        }
    }
}