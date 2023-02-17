using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RSSFeedBot.Enumerations;

namespace RSSFeedBot.Configurations
{
    public class RSSFeedConfiguration
    {
        public FeedSite[] Sites { get; set; }
        public Templates Templates { get; set; }
        public Hashtag[] Hashtags { get; set; }
        public int PostIntervalSeconds { get; set; }
    }

    public class FeedSite
    {
        public string SiteName { get; set; }
        public SiteTypes SiteType { get; set; }
        public string FeedUrl { get; set; }
        public HashtagTypes[] HashtagTypes { get; set; }
    }

    public class Templates
    {
        public string[] Post { get; set; }
    }

    public class Hashtag
    {
        public HashtagTypes Type { get; set; }
        public string Name { get; set; }
    }
}
