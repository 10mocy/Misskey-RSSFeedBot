using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RSSFeedBot.Services
{
    public interface IHTTPClientService
    {
        Task<HttpResponseMessage> GetAsync();
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostWithJsonAsync<T>(string url, T content);
    }

    public class HTTPClientService : IHTTPClientService
    {
        private HttpClient HttpClient = new();

        public HTTPClientService(
            string baseUrl,
            IDictionary<string, string> headers = null)
        {
            //Logger.LogDebug($"----- Set Base URL\n> {0}", baseUrl);
            HttpClient.BaseAddress = new Uri(baseUrl);

            //Logger.LogDebug("----- Set HTTP Headers");
            /*Logger.LogDebug("> No set HTTP Headers");*/
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    //Logger.LogDebug($"> {0}: {1}", header.Key, header.Value);
                }
            }
        }

        public async Task<HttpResponseMessage> GetAsync()
        {
            return await GetAsync("");
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            try
            {
                //Logger.LogDebug($"----- HTTPClientService.GetAsync\n> {0}", url);
                using var response = await HttpClient.GetAsync(url);
                return response;
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        public async Task<HttpResponseMessage> PostWithJsonAsync<T>(string url, T content)
        {
            try
            {
                //Logger.LogDebug($"----- HTTPClientService.PostAsync\n> {0}", url);
                var json = JsonSerializer.Serialize(content);
                var body = new StringContent(json, Encoding.UTF8, @"application/json");
                using var response = await HttpClient.PostAsync(url, body);
                return response;
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }
    }
}
