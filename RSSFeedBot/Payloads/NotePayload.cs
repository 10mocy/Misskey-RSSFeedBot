﻿using System.Text.Json.Serialization;

namespace RSSFeedBot.Payloads
{
    public class NotePayload : IHasAccessToken
    {
        [JsonPropertyName("i")]
        public string AccessToken { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
