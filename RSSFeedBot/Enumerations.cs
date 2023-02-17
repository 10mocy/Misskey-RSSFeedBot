using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedBot
{
    public class Enumerations
    {
        public enum SiteTypes
        {
            RSSFeed = 1,
        }

        public enum HashtagTypes
        {
            Automation = -1,
            SiteName = 0,
            Information = 1,
            News = 2,
            TransportInformation = 3,
        }
    }
}
