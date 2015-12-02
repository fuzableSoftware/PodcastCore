using System.Collections.Generic;

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
        /// Number of episodes to keep
        /// </summary>
        public int EpisodesToKeep { get; set; }
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
        /// <param name="episodesToKeep">Number of episodes to keep</param>
        public Podcast(string name, string url, int episodesToKeep)
        {
            Name = name;
            Url = url;
            EpisodesToKeep = episodesToKeep;
            EpisodesToDownload = new List<Episode>();
            EpisodesToDelete = new List<Episode>();
        }

        /// <summary>
        /// Process a podcast's feed
        /// </summary>
        public void ProcessFeed()
        {
        }
    }
}
