﻿using System.IO;
using System.Net;
using Fuzable.Podcast.Entities.Episodes;

namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// A podcast episode
    /// </summary>
    public class Episode
    {
        /// <summary>
        /// Title of episode
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Episode URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Path to episode
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Episode downloading event
        /// </summary>
        public event EpisodeDownloadingHandler EpisodeDownloading;
        /// <summary>
        /// Raises EpisodeDownloading event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        protected virtual void OnEpisodeDownloading(string name, string url, string path)
        {
            EpisodeDownloading?.Invoke(this, new EpisodeDetailEventArgs(name, url, path));
        }
        /// <summary>
        /// Episode downloaded event
        /// </summary>
        public event EpisodeDownloadedHandler EpisodeDownloaded;
        /// <summary>
        /// Raises EpisodeDownloaded event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Local path to episode</param>
        protected virtual void OnEpisodeDownloaded(string name, string url, string path)
        {
            EpisodeDownloaded?.Invoke(this, new EpisodeDetailEventArgs(name, url, path));
        }
        /// <summary>
        /// Episode download failed event
        /// </summary>
        public event EpisodeDownloadFailedHandler EpisodeDownloadFailed;
        /// <summary>
        /// Raises EpisodeDownloadFailed event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        protected virtual void OnEpisodeDownloadFailed(string name, string url)
        {
            EpisodeDownloadFailed?.Invoke(this, new EpisodeDetailEventArgs(name, url));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Podcast title</param>
        /// <param name="url">Podcast URL</param>
        /// <param name="filepath">Podcast path</param>
        public Episode(string title, string url, string filepath)
        {
            Title = title;
            Url = url;
            FilePath = filepath;
        }

        /// <summary>
        /// Downloads an episode
        /// </summary>
        public void Download()
        {
            if (!File.Exists(FilePath))
            {
                try
                {
                    OnEpisodeDownloading(Title, Url, FilePath);
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(Url, FilePath);
                    }
                    OnEpisodeDownloaded(Title, Url, FilePath);
                }
                catch (WebException)
                {
                    OnEpisodeDownloadFailed(Title, Url);
                    //delete failed download
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                }
            }
        }
    }

}
