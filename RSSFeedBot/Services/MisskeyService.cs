using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RSSFeedBot.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedBot.Services
{
    public interface IMisskeyService
    {
        Task<HttpResponseMessage> PostNoteAsync(string note);
    }

    public class MisskeyService : IMisskeyService
    {
        private HTTPClientService HTTPClientService { get; }
        private string AccessToken { get; }
        private string ChannelId { get; }

        public MisskeyService()
        {
            var misskeyBaseUrl = Environment.GetEnvironmentVariable("MisskeyBaseUrl");
            AccessToken = Environment.GetEnvironmentVariable("MisskeyAccessToken");
            ChannelId = Environment.GetEnvironmentVariable("MisskeyChannelId");

            HTTPClientService = new HTTPClientService(misskeyBaseUrl);
        }

        public async Task<HttpResponseMessage> PostNoteAsync(string text)
        {
            var request = new NotePayload
            {
                AccessToken = AccessToken,
                ChannelId = ChannelId,
                Text = text,
            };
            return await HTTPClientService.PostWithJsonAsync("/api/notes/create", request);
        }
    }
}
