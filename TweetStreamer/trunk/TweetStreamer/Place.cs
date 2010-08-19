using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TweetStreamer
{
    [DataContract]
    public class Place
    {
        [DataMember(Name = "country")]
        public string Country
        {
            get;
            set;
        }

        [DataMember(Name = "url")]
        public string Url
        {
            get;
            set;
        }

        [DataMember(Name = "full_name")]
        public string FullName
        {
            get;
            set;
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        [DataMember(Name = "place_type")]
        public string PlaceType
        {
            get;
            set;
        }

        [DataMember(Name = "id")]
        public string Id
        {
            get;
            set;
        }
    }
}
