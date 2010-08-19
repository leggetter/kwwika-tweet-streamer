using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TweetStreamer
{
    public interface ILogger
    {
        void LogInfo(string message);

        void LogError(string message);
        
        void LogError(Exception ex);
        
        void LogError(string message, Exception ex);
    }
}
