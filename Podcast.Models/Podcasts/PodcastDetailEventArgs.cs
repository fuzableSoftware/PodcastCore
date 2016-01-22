using System;

namespace Fuzable.Podcast.Entities.Podcasts
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
        /// <param name="name">Name of podcast</param>
        public PodcastDetailEventArgs(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor with all information
        /// </summary>
        /// <param name="name">Podcast name</param>
        /// <param name="url">Podcast address</param>
        /// <param name="episodesToDownload">Number of episodes to download</param>
        /// <param name="episodesToDelete">Number of episodes to potentially delete</param>
        public PodcastDetailEventArgs(string name, string url, int episodesToDownload, int episodesToDelete)
        {
            Name = name;
            Url = url;
            EpisodesToDownload = episodesToDownload;
            EpisodesToDelete = episodesToDelete;
        }
    }
}