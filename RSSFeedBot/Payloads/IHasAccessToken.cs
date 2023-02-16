using System.Text.Json.Serialization;

namespace RSSFeedBot.Payloads
{
    public interface IHasAccessToken
    {
        public string AccessToken { get; set; }
    }
}
