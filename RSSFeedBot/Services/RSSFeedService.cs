﻿using RSSFeedBot.Models;
using System;
using System.Collections.Generic;
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
        RSSFeedItem[] GetLatestPostsByCount(string url, SiteTypes siteType, int count);
    }

    public class RSSFeedService : IRSSFeedService
    {
        public RSSFeedItem GetLatestPost(string url, SiteTypes siteType)
        {
            return GetLatestPostsByCount(url, siteType, 1).First();
        }

        public RSSFeedItem[] GetLatestPostsByCount(string url, SiteTypes siteType, int count)
        {
            var feed = XElement.Load(url);
            var posts = siteType switch
            {
                SiteTypes.RSSFeed => ParseRSSFeeds(feed, count),
                _ => throw new ArgumentOutOfRangeException()
            };
            return posts;
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

        private RSSFeedItem[] ParseRSSFeeds(XElement feed, int count)
        {
            var root = feed.Element("channel");
            var items = root.Elements("item")
                .Take(count)
                .ToArray();
            if (items.Length == 0) throw new InvalidDataException();

            var posts = new List<RSSFeedItem>();
            foreach (var item in items)
            {
                var title = item.Element("title").Value;
                var description = item.Element("description").Value;
                var url = item.Element("link").Value;
                var updatedDate = DateTime.Parse(item.Element("pubDate").Value);

                description = Regex.Replace(description, @"<.*?>", string.Empty);
                var descriptionFirstSentence = Regex.Match(description, @"^.+?。").Value;

                var post = new RSSFeedItem
                {
                    Id = GenerateId(url, updatedDate),
                    PostTitle = title,
                    Description = descriptionFirstSentence.Length != 0 ? descriptionFirstSentence : description,
                    Url = url,
                    UpdatedDate = updatedDate,
                };
                posts.Add(post);
            }

            return posts.ToArray();
        }

        /*private RSSFeedItem ParseNote(XElement feed)
        {
            // XNamespace ns = "http://www.nhk.or.jp/rss/rss2.0/modules/nhknews/";
            return new RSSFeedItem();
        }*/
    }

}
