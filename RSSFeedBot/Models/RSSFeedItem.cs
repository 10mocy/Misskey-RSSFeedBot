using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedBot.Models
{
    public class RSSFeedItem
    {
        public string Id { get; set; }

        public string PostTitle { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
