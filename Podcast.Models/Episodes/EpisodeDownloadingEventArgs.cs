using System;

namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Episode detail event arguments
    /// </summary>
    public class EpisodeDownloadingEventArgs : EventArgs
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
        /// Available values for Result when attempting to download episode
        /// </summary>
        public enum EpisodeResult
        {
            /// <summary>
            /// Episode is downloading 
            /// </summary>
            Downloading,
            /// <summary>
            /// Episode is downloaded
            /// </summary>
            Downloaded,
            /// <summary>
            /// Episode could not be downloaded or copied
            /// </summary>
            Failed 
        };

        /// <summary>
        /// Result of episode being downloaded
        /// </summary>
        public EpisodeResult Result { get; set; }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        public EpisodeDownloadingEventArgs(string name, string url)
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
        public EpisodeDownloadingEventArgs(string name, string url, string path)
        {
            Name = name;
            Url = url;
            Path = path;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        /// <param name="result">Result of download attempt</param>
        public EpisodeDownloadingEventArgs(string name, string url, string path, EpisodeResult result)
        {
            Name = name;
            Url = url;
            Path = path;
            Result = result;
        }

    }
}