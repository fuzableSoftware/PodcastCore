using System;

namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Episode detail event arguments
    /// </summary>
    public class EpisodeEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the episode
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Podcast address
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Podcast local path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="name">Episode name</param>
        public EpisodeEventArgs(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        public EpisodeEventArgs(string name, string url)
        {
            Name = name;
            Url = url;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        public EpisodeEventArgs(string name, string url, string path)
        {
            Name = name;
            Url = url;
            Path = path;
        }
    }
}