using System.IO;
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

        #region Events

        /// <summary>
        /// Episode downloading event
        /// </summary>
        public event EpisodeDownloadingHandler EpisodeDownloading;

        /// <summary>
        /// Episode downloaded event
        /// </summary>
        public event EpisodeDownloadedHandler EpisodeDownloaded;

        /// <summary>
        /// Episode download failed event
        /// </summary>
        public event EpisodeDownloadFailedHandler EpisodeDownloadFailed;

        /// <summary>
        /// Episode download synchronized event
        /// </summary>
        public event EpisodeSynchronizedHandler EpisodeSynchronized;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises EpisodeDownloading event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        protected virtual void OnEpisodeDownloading(string name, string url, string path)
        {
            EpisodeDownloading?.Invoke(this, new EpisodeEventArgs(name, url, path));
        }

        /// <summary>
        /// Raises EpisodeDownloaded event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Local path to episode</param>
        protected virtual void OnEpisodeDownloaded(string name,string url, string path)
        {
            EpisodeDownloaded?.Invoke(this, new EpisodeEventArgs(name, url, path));
        }

        /// <summary>
        /// Raises EpisodeDownloadFailed event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        protected virtual void OnEpisodeDownloadFailed(string name, string url)
        {
            EpisodeDownloadFailed?.Invoke(this, new EpisodeEventArgs(name, url));
        }

        /// <summary>
        /// Raises EpisodeSynchronized event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        protected virtual void OnEpisodeSynchronized(string name, string url)
        {
            EpisodeSynchronized?.Invoke(this, new EpisodeEventArgs(name, url));
        }

        #endregion

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
            if (File.Exists(FilePath))
            {
                OnEpisodeSynchronized(Title, null);
            }
            else
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
                finally
                {
                    OnEpisodeSynchronized(Title, Url);
                }
            }
        }
    }
}
