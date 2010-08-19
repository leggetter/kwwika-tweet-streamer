using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TweetStreamer
{
    internal class DataTransferRateChangeEventArgs : IDataTransferRateChangeEventArgs
    {
        public float DataTranferRate
        {
            get;
            set;
        }

        internal DataTransferRateChangeEventArgs(float dataTransferRate)
        {
            DataTranferRate = dataTransferRate;
        }
    }
}
