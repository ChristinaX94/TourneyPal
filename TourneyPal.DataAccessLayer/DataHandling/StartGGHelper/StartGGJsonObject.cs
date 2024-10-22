using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.DataHandling.StartGGHelper
{
    public class StartGGJsonObject
    {
        public class CacheControl
        {
            public int version { get; set; }
            public List<Hint> hints { get; set; }
        }

        public class Data
        {
            public Tournaments tournaments { get; set; }
        }

        public class Event
        {
            public string name { get; set; }
            public Videogame videogame { get; set; }
        }

        public class Extensions
        {
            public CacheControl cacheControl { get; set; }
            public int queryComplexity { get; set; }
        }

        public class Hint
        {
            public List<string> path { get; set; }
            public int maxAge { get; set; }
            public string scope { get; set; }
        }

        public class Node
        {
            public int id { get; set; }
            public string name { get; set; }
            public string countryCode { get; set; }
            public string addrState { get; set; }
            public string city { get; set; }
            public int startAt { get; set; }
            public bool isOnline { get; set; }
            public string url { get; set; }
            public int state { get; set; }
            public string venueAddress { get; set; }
            public string venueName { get; set; }
            public bool isRegistrationOpen { get; set; }
            public int? numAttendees { get; set; }
            public List<Event> events { get; set; }
            public List<Stream> streams { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public Extensions extensions { get; set; }
            public List<object> actionRecords { get; set; }
        }

        public class Stream
        {
            public string streamName { get; set; }
        }

        public class Tournaments
        {
            public List<Node> nodes { get; set; }
        }

        public class Videogame
        {
            public string name { get; set; }
        }
    }
}
