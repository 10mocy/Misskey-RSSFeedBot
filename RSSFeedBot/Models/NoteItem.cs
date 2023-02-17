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
        public string SiteName { get; set; }
        public HashtagTypes[] HashtagTypes { get; set; } = Array.Empty<HashtagTypes>();

        public RSSFeedItem Post { get; set; }
    }
}
