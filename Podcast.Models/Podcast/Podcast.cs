﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Fuzable.Podcast.Entities.Podcast
{
    /// <summary>
    /// an individual podcast
    /// </summary>
    public class Podcast
    {
        /// <summary>
        /// Name of podcast
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// URL of podcast
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Number of episodes to keep
        /// </summary>
        public int EpisodesToKeep { get; set; }
        /// <summary>
        /// Episodes to download during sync
        /// </summary>
        public List<Episode.Episode> EpisodesToDownload { get; set; }
        /// <summary>
        /// Episodes to delete during sync
        /// </summary>
        public List<Episode.Episode> EpisodesToDelete { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Podcast name</param>
        /// <param name="url">Podcast URL</param>
        /// <param name="episodesToKeep">Number of episodes to keep</param>
        public Podcast(string name, string url, int episodesToKeep)
        {
            Name = name;
            Url = url;
            EpisodesToKeep = episodesToKeep;
            EpisodesToDownload = new List<Episode.Episode>();
            EpisodesToDelete = new List<Episode.Episode>();
        }

        /// <summary>
        /// Process a podcast's feed
        /// </summary>
        public void ProcessFeed(string downloadFolder)
        {
            try
            {
                var xmlDoc = XDocument.Load(Url);

                var items = from item in xmlDoc.Descendants("item")
                            select new
                            {
                                Title = item.Element("title")?.Value,
                                Link = item.Element("enclosure")?.Attribute("url").Value
                            };

                EpisodesToDownload.Clear();
                EpisodesToDelete.Clear();

                var counter = 0;
                foreach (var item in items)
                {
                    var filePath = CreateFilePathFromUrl(item.Link, downloadFolder);

                    if (EpisodesToKeep == 0 || counter < EpisodesToKeep)
                    {
                        EpisodesToDownload.Add(new Episode.Episode(item.Title, item.Link, filePath));
                    }
                    else
                    {
                        EpisodesToDelete.Add(new Episode.Episode(item.Title, item.Link, filePath));
                    }
                    counter++;
                }
            }
            catch (WebException webex)
            {
                var error = new ApplicationException($"Problems downloading the feed '{Name}'", webex);
                throw error;
            }
            catch (NullReferenceException nullex)
            {
                var error = new ApplicationException($"Problems parsing the feed '{Name}'", nullex);
                throw error;
            }
        }

        private static string CreateFilePathFromUrl(string fileUrl, string downloadFolder)
        {
            var filename = fileUrl.Split('/').Last();
            return Path.Combine(downloadFolder, filename);
        }
    }
}