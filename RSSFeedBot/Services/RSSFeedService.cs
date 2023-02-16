using RSSFeedBot.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static RSSFeedBot.Enumerations;

namespace RSSFeedBot.Services
{
    public interface IRSSFeedService
    {
        RSSFeedItem GetLatestPost(string url, SiteTypes siteType);
    }

    public class RSSFeedService : IRSSFeedService
    {
        //public RSSFeedService()
        //{
        //    HTTPClientService = new HTTPClientService(misskeyBaseUrl);
        //}

        public RSSFeedItem GetLatestPost(string url, SiteTypes siteType)
        {
            var feed = XElement.Load(url);
            var post = siteType switch
            {
                SiteTypes.RSSFeed => ParseRSSFeed(feed),
                _ => throw new ArgumentOutOfRangeException()
            };
            return post;
        }

        private string GenerateId(string url, DateTime dateTime)
        {
            using var sha = SHA256.Create();
            var bytedId = System.Text.Encoding.ASCII.GetBytes($"{dateTime.ToString("yyyyMMddHHmmss")}-{url}");
            var hashedBytes = sha.ComputeHash(bytedId);

            var hashDigest = new StringBuilder();
            foreach (var hashedByte in hashedBytes)
            {
                hashDigest.Append(hashedByte.ToString("x2"));
            }
            return hashDigest.ToString();
        }

        private RSSFeedItem ParseRSSFeed(XElement feed)
        {
            var root = feed.Element("channel");
            var post = root.Elements("item")
                .FirstOrDefault();
            if (post == null) throw new InvalidDataException();

            var title = post.Element("title").Value;
            var description = post.Element("description").Value;
            var url = post.Element("link").Value;
            var updatedDate = DateTime.Parse(post.Element("pubDate").Value);

            description = Regex.Replace(description, @"<.*?>", string.Empty);
            var descriptionFirstSentence = Regex.Match(description, @".+。").Value;

            return new RSSFeedItem
            {
                Id = GenerateId(url, updatedDate),
                PostTitle = title,
                Description = descriptionFirstSentence.Length != 0 ? descriptionFirstSentence : description,
                Url = url,
                UpdatedDate = updatedDate,
            };
        }

        /*private RSSFeedItem ParseNote(XElement feed)
        {
            // XNamespace ns = "http://www.nhk.or.jp/rss/rss2.0/modules/nhknews/";
            return new RSSFeedItem();
        }*/
    }

}
