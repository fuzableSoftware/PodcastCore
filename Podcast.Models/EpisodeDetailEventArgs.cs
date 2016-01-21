using System;

namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// Episode detail event arguments
    /// </summary>
    public class EpisodeDetailEventArgs : EventArgs
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
        /// Podcast local path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        public EpisodeDetailEventArgs(string name, string url)
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
        public EpisodeDetailEventArgs(string name, string url, string path)
        {
            Name = name;
            Url = url;
            FilePath = path;
        }
    }
}