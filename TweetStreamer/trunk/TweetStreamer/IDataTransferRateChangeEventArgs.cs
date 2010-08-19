using System;

namespace TweetStreamer
{
    public interface IDataTransferRateChangeEventArgs
    {
        float DataTranferRate { get; }
    }
}
