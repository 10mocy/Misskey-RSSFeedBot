using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using RSSFeedBot.Models;
using RSSFeedBot.Services;
using RSSFeedBot.Configurations;
using System.Net.Http;
using static RSSFeedBot.Enumerations;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System;
using Microsoft.Azure.Cosmos.Linq;
using System.Security.Policy;

namespace RSSFeedBot
{
    public class RSSFeed
    {
        public RSSFeed(IOptions<RSSFeedConfiguration> rssFeedConfiguration)
        {
            RSSFeedConfiguration = rssFeedConfiguration;
            MisskeyService = new();
        }

        private IOptions<RSSFeedConfiguration> RSSFeedConfiguration { get; }
        private MisskeyService MisskeyService { get; }

        [FunctionName(nameof(RunWithTimer))]
        public async Task RunWithTimer(
            [TimerTrigger("0 */10 * * * *")]TimerInfo timer,
            [CosmosDB(Connection = "CosmosDBConnection")]CosmosClient cosmosClient)
        {
            await Main(cosmosClient);
        }

        [FunctionName(nameof(RunWithHTTP))]
        public async Task<IActionResult> RunWithHTTP(
            [HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequest req,
            [CosmosDB(Connection = "CosmosDBConnection")]CosmosClient cosmosClient)
        {
            var result = await Main(cosmosClient);
            return new OkObjectResult(result);
        }

        private async Task<NoteItem[]> Main(CosmosClient cosmosClient)
        {
            var sites = RSSFeedConfiguration.Value.Sites;
            var notes = new List<NoteItem>();
            foreach (var site in sites)
            {
                var post = FetchFeed(site.FeedUrl, site.SiteType);
                var note = new NoteItem
                {
                    Id = post.Id,
                    SiteName = site.SiteName,
                    PostTitle = post.PostTitle,
                    Description = post.Description,
                    Url = post.Url
                };
                notes.Add(note);
            }
            if (notes.Count <= 0) return notes.ToArray();

            var fetchedPostsContainer = cosmosClient.GetContainer("RSSFeedBot", "FetchedPosts");
            var fetchedPostIdsIterator = fetchedPostsContainer
                .GetItemLinqQueryable<FetchedPost>()
                .Select(i => i.PostId)
                .ToFeedIterator();
            var fetchedPostIds = new List<string>();
            while (fetchedPostIdsIterator.HasMoreResults)
            {
                foreach (var fetchedPostId in await fetchedPostIdsIterator.ReadNextAsync())
                {
                    fetchedPostIds.Add(fetchedPostId);
                }
            }

            var createDocumentTasks = new List<Task>();
            var noteQueue = new List<NoteItem>();
            foreach (var note in notes)
            {
                if (fetchedPostIds.Contains(note.Id)) continue;
                var fetchedPost = new FetchedPost
                {
                    Id = Guid.NewGuid().ToString(),
                    PostId = note.Id,
                };
                noteQueue.Add(note);
                createDocumentTasks.Add(fetchedPostsContainer.CreateItemAsync(fetchedPost, new PartitionKey(note.Id)));
            }

            foreach (var note in noteQueue)
            {
                await PostNoteByInterval(note);
            }

            await Task.WhenAll(createDocumentTasks);
            return noteQueue.ToArray();
        }

        private static RSSFeedItem FetchFeed(string url, SiteTypes siteType)
        {
            var rssFeedService = new RSSFeedService();
            return rssFeedService.GetLatestPost(url, siteType);
        }

        private async Task<HttpResponseMessage> PostNoteByInterval(NoteItem note)
        {
            var template = string.Join('\n', RSSFeedConfiguration.Value.Templates.Post);
            var message = template
                .Replace(":SiteName", note.SiteName)
                .Replace(":PostTitle", note.PostTitle)
                .Replace(":Description", note.Description)
                .Replace(":Url", note.Url);

            var result = await MisskeyService.PostNoteAsync(message);

            var interval = RSSFeedConfiguration.Value.PostIntervalSeconds;
            await Task.Delay(interval * 1000);

            return result;
        }

    }
}
