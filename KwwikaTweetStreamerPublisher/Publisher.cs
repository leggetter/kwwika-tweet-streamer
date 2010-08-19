using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TweetStreamer;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Kwwika;
using System.Threading;

namespace KwwikaTweetStreamerPublisher
{
    public class Publisher: IConnectionListener, ICommandListener
    {
        TweetStreamer.Connection _connection;
        IConnection _kwwikaConnection;
        private TweetStreamer.ConnectionStatus _actualConnectionStatus = TweetStreamer.ConnectionStatus.Disconnected;
        private TweetStreamer.ConnectionStatus _intendedConnectionStatus = TweetStreamer.ConnectionStatus.Disconnected;
        private KwwikaTweetStreamerPublisherConfig _config;
        private Dictionary<string, long> _countFields = new Dictionary<string, long>();
        private LoggerWrapper _logger;

        public Publisher(KwwikaTweetStreamerPublisherConfig config)
        {
            KwwikaTweetStreamerPublisherConfig.CheckConfiguration(config);

            _logger = new LoggerWrapper();

            _config = config;

            string twitterParams = BuildTwitterParams(config.SearchDefinitions);

            _connection = new TweetStreamer.Connection(config.TwitterConfig.Username, config.TwitterConfig.Password, config.TwitterConfig.Url, twitterParams, "POST", _logger);
            _connection.ConnectionStatusChanged += new TweetStreamer.Connection.OnConnectionStatusChangedEventHandler(connection_ConnectionStatusChanged);
            _connection.StatusMessageReceived += new TweetStreamer.Connection.OnStatusMessageReceivedEventHandler(connection_StatusMessageReceived);
        }

        private string BuildTwitterParams(SearchDefinition[] searchDefinitions)
        {
            List<string> tracks = new List<string>();
            List<string> userIds = new List<string>();

            // TODO: other types of search e.g. follow
            foreach (SearchDefinition def in searchDefinitions)
            {
                if (def.Request == true)
                {
                    if (string.IsNullOrEmpty(def.TrackFor) == false)
                    {
                        string[] configuredTracks = def.TrackFor.Split(',');
                        foreach (string track in configuredTracks)
                        {
                            if (tracks.Contains(track) == false)
                            {
                                tracks.Add(track);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(def.FollowUsersWithId) == false)
                    {
                        string[] configuredIds = def.FollowUsersWithId.Split(',');
                        foreach (string id in configuredIds)
                        {
                            if (configuredIds.Contains(id) == false)
                            {
                                userIds.Add(id);
                            }
                        }
                        userIds.AddRange(configuredIds);
                    }
                }
            }
            string twitterParameters = String.Join(",", tracks.ToArray());
            if(string.Empty != twitterParameters)
            {
                twitterParameters = "track=" + twitterParameters;
            }
            string userIdsToFollow = String.Join(",", userIds.ToArray());
            if (string.Empty != userIdsToFollow)
            {
                if (twitterParameters != string.Empty)
                {
                    twitterParameters += "&";
                }
                twitterParameters += "follow=" + userIdsToFollow;
            }
            return twitterParameters;
        }

        public bool Connected
        {
            get
            {
                return _actualConnectionStatus == TweetStreamer.ConnectionStatus.Connected;
            }
        }

        public void Connect()
        {
            _intendedConnectionStatus = TweetStreamer.ConnectionStatus.Connected;
            _kwwikaConnection = Kwwika.Service.Connect(_config.KwwikaConfig.ApiKey, _config.KwwikaConfig.Domain, this);
            _kwwikaConnection.Logger = _logger;
            this.SendIntialCountFieldValues(_config.SearchDefinitions);
        }

        internal void Disconnect()
        {
            _intendedConnectionStatus = TweetStreamer.ConnectionStatus.Disconnected;
            _connection.Disconnect();
            if (_kwwikaConnection != null)
            {
                _kwwikaConnection.Disconnect();
            }
        }

        private void AddFieldOrBlankToDictionary(Dictionary<string, string> tweetUpdate, string name, string value)
        {
            value = String.IsNullOrEmpty(value) ? "" : value;
            tweetUpdate.Add(name, value);
        }

        #region TweetStreamer events
        private Timer _connectionTimer;
        void connection_ConnectionStatusChanged(object sender, IConnectionStatusChangedEventArgs args)
        {
            _actualConnectionStatus = args.Status;

            if (_intendedConnectionStatus == TweetStreamer.ConnectionStatus.Connected &&
                _actualConnectionStatus == TweetStreamer.ConnectionStatus.Disconnected)
            {
                if (_connectionTimer != null)
                {
                    _connectionTimer.Dispose();
                    _connectionTimer = null;
                }

                int milliSecondsToReconnect = GetSleepMilliSeconds();
                _logger.Log(Kwwika.Logging.LogLevels.Info, "Connection", "Waiting {0} milliseconds before connecting.", milliSecondsToReconnect);
                _connectionTimer = new Timer(new TimerCallback((object state) =>
                {
                    _connection.Connect();
                    _connectionTimer = null;
                }), null, milliSecondsToReconnect, Timeout.Infinite);
            }

            if (args.Status == TweetStreamer.ConnectionStatus.Connected)
            {
                ResetSleepMilliSeconds();
            }
        }

        private int _sleepMilliSeconds = 0;
        private void ResetSleepMilliSeconds()
        {
            _sleepMilliSeconds = 0;
        }

        private int GetSleepMilliSeconds()
        {
            int millis = _sleepMilliSeconds;
            if (_sleepMilliSeconds == 0)
            {
                _sleepMilliSeconds = 5000;
            }
            _sleepMilliSeconds = Math.Min((_sleepMilliSeconds * 2), 240000);
            return millis;
        }

        void connection_StatusMessageReceived(object sender, IStatusMessageReceivedEventArgs args)
        {
            var tweetUpdate = new Dictionary<string, string>();
            AddFieldOrBlankToDictionary(tweetUpdate, "CreatedAt" ,args.Message.CreatedAt.ToString());
            AddFieldOrBlankToDictionary(tweetUpdate, "Favourited", args.Message.Favourited?"true":"false");
            AddFieldOrBlankToDictionary(tweetUpdate, "Id", args.Message.Id);
            AddFieldOrBlankToDictionary(tweetUpdate, "InReplyToScreenName", args.Message.InReplyToScreenName);
            AddFieldOrBlankToDictionary(tweetUpdate, "InReplyToStatusId", args.Message.InReplyToStatusId);
            AddFieldOrBlankToDictionary(tweetUpdate, "InReplyToUserId", args.Message.InReplyToUserId);
            AddFieldOrBlankToDictionary(tweetUpdate, "Source", args.Message.Source);
            AddFieldOrBlankToDictionary(tweetUpdate, "Text", args.Message.Text);
            AddFieldOrBlankToDictionary(tweetUpdate, "Truncated", args.Message.Truncated ? "true" : "false");
            AddFieldOrBlankToDictionary(tweetUpdate, "UserFollowersCount", args.Message.User.FollowersCount.ToString());
            AddFieldOrBlankToDictionary(tweetUpdate, "UserName", args.Message.User.Name);
            AddFieldOrBlankToDictionary(tweetUpdate, "UserProfileImageUrl", args.Message.User.ProfileImageUrl);
            AddFieldOrBlankToDictionary(tweetUpdate, "ScreenName", args.Message.User.ScreenName);
            
            // Location stuff
            Place place = args.Message.Place ?? new Place();
            Geo geo = args.Message.Geo ?? new Geo();
            AddFieldOrBlankToDictionary(tweetUpdate, "PlaceCountry", place.Country);
            AddFieldOrBlankToDictionary(tweetUpdate, "PlaceFullName", place.FullName);
            AddFieldOrBlankToDictionary(tweetUpdate, "PlaceId", place.Id);
            AddFieldOrBlankToDictionary(tweetUpdate, "PlaceName", place.Name);
            AddFieldOrBlankToDictionary(tweetUpdate, "PlaceType", place.PlaceType);
            AddFieldOrBlankToDictionary(tweetUpdate, "PlaceUrl", place.Url);
            
            AddFieldOrBlankToDictionary(tweetUpdate, "GeoType", geo.Type);
            if (geo.Type == "Point")
            {
                try
                {
                    // Note: Twitter v1 API sends Lat,Long. v2 will send Long,Lat
                    decimal[] coords = (decimal[])args.Message.Geo.Coordinates;
                    AddFieldOrBlankToDictionary(tweetUpdate, "GeoLat", ((decimal)coords[0]).ToString());
                    AddFieldOrBlankToDictionary(tweetUpdate, "GeoLong", ((decimal)coords[1]).ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError("error getting Geo Cordinates " + ex.ToString());
                }
            }
            else
            {
                AddFieldOrBlankToDictionary(tweetUpdate, "GeoLat", "");
                AddFieldOrBlankToDictionary(tweetUpdate, "GeoLong", "");
            }
            
            string[] topics = DetermineTopicFromTwitterMessage(tweetUpdate, args.Message, _config.SearchDefinitions);
            string[] fieldsToSend = DetermineFieldsToSend(tweetUpdate, _config.SearchDefinitions);
            string[] allFieldsInTweet = tweetUpdate.Keys.ToArray<string>();

            AddCountFieldsToUpdate(topics, tweetUpdate, _config.SearchDefinitions);

            foreach (string fieldInTweet in allFieldsInTweet)
            {
                if (fieldsToSend.Contains<string>(fieldInTweet) == false)
                {
                    tweetUpdate.Remove(fieldInTweet);
                }
            }

            if (tweetUpdate.Keys.Count > 0)
            {
                foreach (string topic in topics)
                {
                    _logger.LogInfo("publishing to " + topic);
                    Publish(topic, tweetUpdate);
                }
            }
            else
            {
                _logger.LogInfo("Ignoring tweet. Either no interest in tweet (ignored) or no fields of interest were present");
            }
        }

        public static string[] DetermineFieldsToSend(Dictionary<string, string> tweetUpdate, SearchDefinition[] searchDefinitions)
        {
            List<string> fields = new List<string>();
            foreach (SearchDefinition definition in searchDefinitions)
            {
                if (definition.ShouldIgnoreTweet(tweetUpdate))
                {
                    continue;
                }

                if (definition.FieldsToSend != null)
                {
                    AddFieldIfAbsent(definition.FieldsToSend.Split(','), fields);
                }
                else
                {
                    // add all
                    AddFieldIfAbsent(tweetUpdate.Keys.ToArray<string>(), fields);
                }
            }
            return fields.ToArray();
        }

        private static void AddFieldIfAbsent(string[] fieldsToCheckAndAdd, List<string> fields)
        {
            foreach (string fieldName in fieldsToCheckAndAdd)
            {
                if (fields.Contains(fieldName) == false)
                {
                    fields.Add(fieldName);
                }
            }
        }

        private void Publish(string topic, Dictionary<string,string> publishParams)
        {
            _kwwikaConnection.Publish(topic, publishParams, this);
        }

        private void AddCountFieldsToUpdate(string[] topics, Dictionary<string, string> tweetUpdate, SearchDefinition[] searchDefinitions)
        {
            foreach (SearchDefinition def in searchDefinitions)
            {
                if (topics.Contains(def.PublishTo) && def.CountFields != null)
                {
                    foreach (CountField field in def.CountFields)
                    {
                        if (FieldAlreadyAddedToUpdate(tweetUpdate, field.Name) == false)
                        {
                            long currentValue = (_countFields.ContainsKey(field.Name) == false ? 0 : _countFields[field.Name]);
                            if (field.MatchAll == true)
                            {
                                currentValue++;
                                tweetUpdate.Add(field.Name, currentValue.ToString());
                            }
                            else if (string.IsNullOrEmpty(field.Match.Expression) == false &&
                                string.IsNullOrEmpty(field.Match.FieldToMatch) == false &&
                                tweetUpdate.ContainsKey(field.Match.FieldToMatch) == true)
                            {
                                RegexOptions options = (field.Match.IgnoreCase?RegexOptions.IgnoreCase:RegexOptions.None);
                                Regex regex = new Regex(field.Match.Expression, options);
                                string fieldToMatch = tweetUpdate[field.Match.FieldToMatch];
                                if (regex.IsMatch(fieldToMatch) == true)
                                {
                                    currentValue++;
                                    tweetUpdate.Add(field.Name, currentValue.ToString());
                                }
                            }
                            _countFields[field.Name] = currentValue;
                        }
                    }
                }
            }
        }

        private void SendIntialCountFieldValues(SearchDefinition[] searchDefinitions)
        {            
            foreach (SearchDefinition def in searchDefinitions)
            {
                if (def.CountFields != null)
                {
                    Dictionary<string, string> tweetUpdate = new Dictionary<string, string>();
                    foreach (CountField field in def.CountFields)
                    {
                        _countFields[field.Name] = 0;
                        tweetUpdate[field.Name] = "0";
                    }
                    Publish(def.PublishTo, tweetUpdate);
                }
            }
        }

        private static bool FieldAlreadyAddedToUpdate(Dictionary<string, string> tweetUpdate, string name)
        {
            return tweetUpdate.ContainsKey(name);
        }

        public static string[] DetermineTopicFromTwitterMessage(IDictionary<string,string> tweetUpdate, IStatusMessage statusMessage, SearchDefinition[] searchDefinitions)
        {
            List<string> topics = new List<string>();
            foreach (SearchDefinition definition in searchDefinitions)
            {
                if (definition.ShouldIgnoreTweet(tweetUpdate))
                {
                    continue;
                }

                if (definition.IsTrackingFor(statusMessage) &&
                    topics.Contains(definition.PublishTo) == false)
                {
                    topics.Add(definition.PublishTo);
                }

                if (definition.IsFollowingUser(statusMessage.User) &&
                    topics.Contains(definition.PublishTo) == false)
                {
                    topics.Add(definition.PublishTo);
                }
            }
            return topics.ToArray();
        }

        #endregion

        #region IConnectionListener Members

        public void ConnectionStatusUpdated(Kwwika.ConnectionStatus status)
        {
            if (status != Kwwika.ConnectionStatus.Connected
                && _intendedConnectionStatus == TweetStreamer.ConnectionStatus.Connected &&
                _connection.Status != TweetStreamer.ConnectionStatus.Connected &&
                _connection.Status != TweetStreamer.ConnectionStatus.Connecting)
            {
                _connection.Connect();
            }
            else
            {
                // TODO: pause Twitter connection updates
            }
        }

        #endregion

        #region ICommandListener Members

        public void CommandError(string topic, CommandErrorType code)
        {
            _logger.LogError("failed to publish to" + topic);
        }

        public void CommandSuccess(string topic)
        {
            _logger.LogInfo("successfully to publish to" + topic);
        }

        #endregion
    }
}
