using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Fuzable.Podcast.Entities
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
        /// Number of episodes to download
        /// </summary>
        public int Download { get; set; }

        /// <summary>
        /// String to remove from title (ex: prefix from vendor)
        /// </summary>
        public string RemoveFromTitle { get; set; }

        /// <summary>
        /// start of titles we don't want
        /// </summary>
        public string ExcludeTitle { get; set; }

        /// <summary>
        /// Possible values for episode order (recent first, all in order, etc.)
        /// </summary>
        public enum EpisodeOrder
        {
            /// <summary>
            /// Most recent podcast first (default)
            /// </summary>
            Recent, 
            /// <summary>
            /// First podcast first; ordered, sequential
            /// </summary>
            Chronological
        }

        /// <summary>
        /// Order to write podcasts to USB key
        /// </summary>
        public EpisodeOrder Order { get; set; }

        /// <summary>
        /// Episodes to download during sync
        /// </summary>
        public List<Episode> EpisodesToDownload { get; set; }
        /// <summary>
        /// Episodes to delete during sync
        /// </summary>
        public List<Episode> EpisodesToDelete { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Podcast name</param>
        /// <param name="url">Podcast URL</param>
        /// <param name="download">Number of episodes to keep</param>
        /// <param name="order">one of EpisodeOrder</param>
        /// <param name="remove">string to remove from episode titles</param>
        /// <param name="exclude">episodes starting with this string will be excluded</param>
        public Podcast(string name, string url, int download, EpisodeOrder order, string remove, string exclude)
        {
            Name = name;
            Url = url;
            Download = download;
            EpisodesToDownload = new List<Episode>();
            EpisodesToDelete = new List<Episode>();
            Order = order;
            RemoveFromTitle = remove;
            ExcludeTitle = exclude;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of podcast</param>
        public Podcast(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Process a podcast's feed
        /// </summary>
        public void ProcessFeed(Subscription subscription)
        {
            try
            {
                //append podcast name to path and verify path exists
                var downloadFolder = subscription.DownloadFolder;
                downloadFolder = Path.Combine(downloadFolder, Name);
                subscription.VerifyFolderExists(downloadFolder);

                var xmlDoc = XDocument.Load(Url);

                var excludeTitlesThatStartWith = "***NO EXCLUSIONS***";
                if (ExcludeTitle != null)
                {
                    excludeTitlesThatStartWith = ExcludeTitle;
                }

                var items = from item in xmlDoc.Descendants("item")
                            where item.Element("title")?.Value.StartsWith(excludeTitlesThatStartWith) == false
                            select new
                            {
                                Title = item.Element("title")?.Value,
                                Link = item.Element("enclosure")?.Attribute("url").Value
                            };

                EpisodesToDownload.Clear();
                EpisodesToDelete.Clear();

                if (Order == EpisodeOrder.Chronological)
                {
                    items = items.Reverse();
                }

                var counter = 0;
                foreach (var item in items)
                {
                    var filePath = "";
                    if (Download == 1 || Download == 0)
                    {
                        //if downloading all available or only one, don't use a number prefix
                        filePath = Episode.GenerateEpisodeFilename(item.Title, downloadFolder, -1, RemoveFromTitle);
                    }
                    else
                    {
                        filePath = Episode.GenerateEpisodeFilename(item.Title, downloadFolder, counter + 1, RemoveFromTitle);
                    }

                    if (Download == 0 || counter < Download)
                    {
                        EpisodesToDownload.Add(new Episode(item.Title, item.Link, filePath));
                    }
                    else
                    {
                        EpisodesToDelete.Add(new Episode(item.Title, item.Link, filePath));
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

        internal static string GetPodcastPath(string path, string podcastName)
        {
            var fullPath = "";
            fullPath = Path.Combine(path, podcastName);
            return fullPath;
        }
    }
}
