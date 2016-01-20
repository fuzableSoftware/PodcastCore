using System;

namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// Podcast detail event arguments
    /// </summary>
    public class PodcastDetailEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the podcast
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Podcast address
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Count of episodes to download
        /// </summary>
        public int EpisodesToDownload { get; set; }

        /// <summary>
        /// Count of episodes to delete
        /// </summary>
        public int EpisodesToDelete { get; set; }

        /// <summary>
        /// Constructor with name only
        /// </summary>
        /// <param name="name"></param>
        public PodcastDetailEventArgs(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor with all information
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="episodesToDownload"></param>
        /// <param name="episodesToDelete"></param>
        public PodcastDetailEventArgs(string name, string url, int episodesToDownload, int episodesToDelete)
        {
            Name = name;
            Url = url;
            EpisodesToDownload = episodesToDownload;
            EpisodesToDelete = episodesToDelete;
        }
    }
}