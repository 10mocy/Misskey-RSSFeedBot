using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedBot.Models
{
    public class FetchedPost
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partionKey")]
        public string PostId { get; set; }

        [JsonProperty("url")]
        public string PostUrl { get; set; }
    }
}
