using System;

namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Episode copy event arguments
    /// </summary>
    public class EpisodeCopyEventArgs : EventArgs
    {
        /// <summary>
        /// Podcast address
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Podcast local path
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="source">Episode name, source file</param>
        /// <param name="destination">destination for Episode (file) copy</param>
        public EpisodeCopyEventArgs(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }
    }
}