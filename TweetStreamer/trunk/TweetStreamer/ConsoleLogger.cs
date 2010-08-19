using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TweetStreamer
{
    public class ConsoleLogger: ILogger
    {
        #region ILogger Members

        public void LogInfo(string message)
        {
            Console.WriteLine("INFO: " + message);
        }

        public void LogError(string message)
        {
            Console.WriteLine("ERROR: " + message);
        }

        public void LogError(Exception ex)
        {
            LogError(ex.ToString());
        }

        public void LogError(string message, Exception ex)
        {
            LogError(message + " " + ex.ToString());
        }

        #endregion
    }
}
