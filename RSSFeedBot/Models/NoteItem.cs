using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RSSFeedBot.Enumerations;

namespace RSSFeedBot.Models
{
    public class NoteItem
    {
        public string Id { get; set; }
        public string SiteName { get; set; }
        public string PostTitle { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public HashtagTypes[] HashtagTypes { get; set; } = Array.Empty<HashtagTypes>();
    }
}
