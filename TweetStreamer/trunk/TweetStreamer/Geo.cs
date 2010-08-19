using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TweetStreamer
{
    [DataContract]
    public class Geo
    {
        [DataMember(Name = "type")]
        public string Type
        {
            get;
            set;
        }

        [DataMember(Name = "coordinates")]
        public decimal[] Coordinates
        {
            get;
            set;
        }
    }
}
