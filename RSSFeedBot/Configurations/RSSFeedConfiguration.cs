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
        public string TimerSchedule { get; set; }
        public int PostIntervalSeconds { get; set; }
    }

    public class FeedSite
    {
        public SiteTypes SiteType { get; set; }
        public string SiteName { get; set; }
        public string FeedUrl { get; set; }
    }

    public class Templates
    {
        public string[] Post { get; set; }
    }
}
