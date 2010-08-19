using System;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace TweetStreamer
{
    /// <summary>
    /// Represents a streaming connection to a Twitter real-time data feed.
    /// </summary>
    /// <remarks>
    /// By default the Twitter sample feed is connected to.
    /// </remarks>
    public class Connection
    {
        /// <summary>
        /// The URL to request.
        /// </summary>
        private string _url = null;

        private string _requestParameters = "";

        private string _requestMethod = "GET";

        /// <summary>
        /// The event handler used whenever a status message is received.
        /// </summary>
        /// <param name="sender">The connection object</param>
        /// <param name="args">Event arguments providing access to the Twitter message (tweet)</param>
        public delegate void OnStatusMessageReceivedEventHandler(object sender, IStatusMessageReceivedEventArgs args);

        /// <summary>
        /// The event that occurs whenever a Twitter message is received by the connection.
        /// </summary>
        public event OnStatusMessageReceivedEventHandler StatusMessageReceived;

        /// <summary>
        /// The event handler used whenever new information is available on the status of the connection to the
        /// Twitter real-time data stream.
        /// </summary>
        /// <param name="sender">The connection object</param>
        /// <param name="args">Event arguments providing access to the connection information.</param>
        public delegate void OnConnectionStatusChangedEventHandler(object sender, IConnectionStatusChangedEventArgs args);

        /// <summary>
        /// The event that occurs whenver new information is available on the status of the connection to the
        /// Twitter real-time data stream. 
        /// </summary>
        public event OnConnectionStatusChangedEventHandler ConnectionStatusChanged;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void OnDataTransferRateChangedHandler(object sender, IDataTransferRateChangeEventArgs args);

        /// <summary>
        /// The event that occurs whenver new information is available on the status of the connection to the
        /// Twitter real-time data stream. 
        /// </summary>
        public event OnDataTransferRateChangedHandler DataTransferRateChanged;


        private int _dateReceivedInLastSecond = 0;
        private float _dataTransferRate = 0;
        private Timer _dataTransferRateMeasurer;
        private Thread _requestThread;

        #region State variables
        private ConnectionStatus _status = ConnectionStatus.Disconnected;
        private StringBuilder _dataBuffer = new StringBuilder();
        #endregion        

#if SILVERLIGHT
        /// <summary>
        /// Creates a new connection to the Twitter real-time data stream.
        /// </summary>
        public Connection(string url)
        {
            this._url = url;
            this._requestParameters = requestParameters;
            this._requestMethod = requestMethod;
        }
#else
        private string _username = null;
        private string _password = null;
        private ILogger _logger;

        /// <summary>
        /// Creates a new connection to the Twitter real-time data stream.
        /// </summary>
        /// <param name="username">A valid Twitter username</param>
        /// <param name="password">The password associated with the provided Twitter username</param>       
        /// <param name="url">The twitter streaming api URL to request. This could contain a query.</param>    
        public Connection(string username, string password, string url, string requestParameters, string requestMethod)
            :this(username, password, url, requestParameters, requestMethod, new ConsoleLogger())
        {
        }

        /// <summary>
        /// Creates a new connection to the Twitter real-time data stream.
        /// </summary>
        /// <param name="username">A valid Twitter username</param>
        /// <param name="password">The password associated with the provided Twitter username</param>       
        /// <param name="url">The twitter streaming api URL to request. This could contain a query.</param>    
        /// <param name="logger">Used to log information about the connection</param>
        public Connection(string username, string password, string url, string requestParameters, string requestMethod, ILogger logger)
        {
            this._url = url;
            this._username = username;
            this._password = password;
            this._requestParameters = requestParameters;
            this._requestMethod = requestMethod;
            this._logger = logger;

            StatusMessage.Logger = _logger;
        }
#endif
        #region Properties
        /// <summary>
        /// The current connection status.
        /// </summary>
        public ConnectionStatus Status
        {
            get
            {
                return this._status;
            }
        }

        /// <summary>
        /// Property to represent the connection status. Setting the property invokes the OnConnectionStatusChanged event.
        /// </summary>
        private ConnectionStatus InternalConnectionStatus
        {
            get
            {
                return this._status;
            }
            set
            {
                this._status = value;
                this.OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(this._status));
            }
        }

        public float DataTransferRate
        {
            get
            {
                return this._dataTransferRate;
            }
            private set
            {
                this._dataTransferRate = value;
                OnDataTransferRateChanged(this._dataTransferRate);
            }
        }

        #endregion

        /// <summary>
        /// Starts the connection to the Twitter real-time data stream.
        /// </summary>
        public void Connect()
        {
            this.InternalConnectionStatus = ConnectionStatus.Connecting;

            try
            {
                _requestThread = new Thread(delegate()
                {
                    HttpWebRequest request = CreateRequest(this._url, this._requestMethod, this._requestParameters);

#if SILVERLIGHT
                request.AllowReadStreamBuffering = false;
#else
                    request.Credentials = new NetworkCredential(this._username, this._password);
#endif

                    request.BeginGetResponse(ConnectionResponseCallback, request);
                });
                _requestThread.Start();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex);
                this.InternalConnectionStatus = ConnectionStatus.Disconnected;
            }

            StartDataTransferRateMeasure();
        }

        private static HttpWebRequest CreateRequest(string url, string requestMethod, string requestParameters)
        {
            var builder = new UriBuilder(url);
            if (requestMethod == "GET" &&
                String.IsNullOrEmpty(requestParameters) == false)
            {
                builder.Query = requestParameters;
            }

            var request = (HttpWebRequest)WebRequest.Create(builder.Uri);
            request.Method = requestMethod;
            if (request.Method == "POST" &&
                String.IsNullOrEmpty(requestParameters) == false)
            {
                AddRequestParametersToPostRequest(requestParameters, request);
            }

            return request;
        }

        private static void AddRequestParametersToPostRequest(string queryParameters, HttpWebRequest request)
        {
            Stream stream = null;

#if !SILVERLIGHT
            request.ContentLength = queryParameters.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            using (stream = request.GetRequestStream())
            {
#else
                EventWaitHandle waitHandle = new AutoResetEvent(false);
                httpRequest.BeginGetRequestStream(delegate(IAsyncResult result)
                {
                    stream = httpRequest.EndGetRequestStream(result);
                    waitHandle.Set();
                }, null);
                bool signalled = waitHandle.WaitOne(10000);
                if (signalled == false)
                {
                    throw new Exception("Could not get Silverlight HTTP request stream");
                }
#endif

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(queryParameters);
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
        }

        private void StartDataTransferRateMeasure()
        {
            this._dataTransferRateMeasurer = new Timer(new TimerCallback(TimeElapsed), null, 1000, 1000);
        }

        private void StopDataTransferRateMeasure()
        {
            this._dataTransferRateMeasurer.Dispose();
        }

        public void TimeElapsed(Object state)
        {
            this.DataTransferRate = this._dateReceivedInLastSecond;
            this._dateReceivedInLastSecond = 0;
        }        

        /// <summary>
        /// Stops the connection to the real-time data stream.
        /// </summary>
        /// <remarks>
        /// Any pending status messages will first be parsed and sent to event listeners.
        /// </remarks>
        public void Disconnect()
        {
            this.InternalConnectionStatus = ConnectionStatus.Disconnecting;
            if (_requestThread != null)
            {
                _requestThread.Join();
            }
            this.StopDataTransferRateMeasure();
        }

        /// <summary>
        /// Called when the initial connection has been established.
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void ConnectionResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        this.InternalConnectionStatus = ConnectionStatus.Connected;

                        this.ReadResponseStream(request, response);

                        if (this.InternalConnectionStatus != ConnectionStatus.Disconnecting)
                        {
                            // unexpected status
                            _logger.LogError("unexpected connection status: " + this.InternalConnectionStatus);
                        }

                    }
                    else
                    {
                        _logger.LogError("unexpected status code: " + response.StatusCode);
                    }
                }

                this.InternalConnectionStatus = ConnectionStatus.Disconnected;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                this.InternalConnectionStatus = ConnectionStatus.Disconnected;
            }
        }

        /// <summary>
        /// Reads the information received from the Twitter real-time data stream.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        private void ReadResponseStream(HttpWebRequest request, HttpWebResponse response)
        {
            byte[] buffer = new byte[65536];
            using (Stream stream = response.GetResponseStream())
            {
                while (this.InternalConnectionStatus == ConnectionStatus.Connected)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);

                    this._dateReceivedInLastSecond += read;

                    UTF8Encoding encoding = new UTF8Encoding();
                    string data = encoding.GetString(buffer, 0, read);
                    ParseResponseChunk(data);
                }

                // need to call request.Abort or the the thread will block at the end of
                // the using block.
                request.Abort();
            }
        }

        /// <summary>
        /// Parses the provided data into individual data messages and initiates the sending of the events
        /// to any event listener.
        /// </summary>
        /// <param name="data">Twitter data. The data may contain between 0 or more messages.</param>
        private void ParseResponseChunk(string data)
        {
            this._dataBuffer.Append(data);

            string allData = this._dataBuffer.ToString();

            // carriage return (\r) indicates the end of a message somewhere in the string
            if (StatusMessage.StringContainsFullMessage(allData) == true)
            {                
                this._dataBuffer = new StringBuilder();

                string leftOverData = StatusMessage.CheckForExcessData(allData);
                if( string.IsNullOrEmpty(leftOverData) == false )
                {
                    // put any data after the last carriage return back onto the buffer
                    this._dataBuffer.Append(leftOverData);

                    allData = allData.Substring(0, allData.Length - leftOverData.Length);
                }

                IStatusMessage[] messages = StatusMessage.CreateMessagesFromJsonString(allData);
                SendMessageEvents(messages);
            }
                
        }

        /// <summary>
        /// Sends each message individually in an event.
        /// </summary>
        /// <param name="messages"></param>
        private void SendMessageEvents(IStatusMessage[] messages)
        {
            foreach (IStatusMessage message in messages)
            {
                OnStatusMessageReceived(new StatusMessageReceivedEventArgs( message ));
            }
        }

        #region Event wrappers
        /// <summary>
        /// Facilitate the sending of status message events.
        /// </summary>
        /// <param name="args"></param>
        private void OnStatusMessageReceived(IStatusMessageReceivedEventArgs args)
        {
            if( this.StatusMessageReceived != null )
            {
                try
                {
                    this.StatusMessageReceived(this, args);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Facilitates the sending of connection status information events.
        /// </summary>
        /// <param name="args"></param>
        private void OnConnectionStatusChanged(IConnectionStatusChangedEventArgs args)
        {
            if (this.ConnectionStatusChanged != null)
            {
                try
                {
                    this.ConnectionStatusChanged(this, args);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
            }
        }


        private void OnDataTransferRateChanged(float dataTransferRate)
        {
            if (this.DataTransferRateChanged != null)
            {
                try
                {
                    var args = new DataTransferRateChangeEventArgs(dataTransferRate);
                    this.DataTransferRateChanged(this, args);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
            }
        }
        #endregion
    }
}
