using System;

namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Episode detail event arguments
    /// </summary>
    public class EpisodeDetailEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the episode
        /// </summary>
        public string PodcastName { get; set; }

        /// <summary>
        /// Podcast address
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Podcast local path
        /// </summary>
        public string DownloadPath { get; set; }
        /// <summary>
        /// Podcast destination (copy) path
        /// </summary>
        public string DestinationPath { get; set; }

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
            Failed, 
            /// <summary>
            /// Episode already exists, no need to download or copy
            /// </summary>
            Exists
        };

        /// <summary>
        /// Result of episode being downloaded
        /// </summary>
        public EpisodeResult Result { get; set; }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="podcastName">Episode name</param>
        /// <param name="url">Episode address</param>
        public EpisodeDetailEventArgs(string podcastName, string url)
        {
            PodcastName = podcastName;
            Url = url;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="podcastName">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        public EpisodeDetailEventArgs(string podcastName, string url, string path)
        {
            PodcastName = podcastName;
            Url = url;
            DownloadPath = path;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="podcastName">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="source">Episode local path, source destination</param>
        /// <param name="destination">Episode local path, copy destination</param>
        /// <param name="result">Result of download attempt</param>
        public EpisodeDetailEventArgs(string podcastName, string url, string source, string destination, EpisodeResult result)
        {
            PodcastName = podcastName;
            Url = url;
            DownloadPath = source;
            DestinationPath = destination;
            Result = result;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="podcastName">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        /// <param name="result">Result of download attempt</param>
        public EpisodeDetailEventArgs(string podcastName, string url, string path, EpisodeResult result)
        {
            PodcastName = podcastName;
            Url = url;
            DownloadPath = path;
            Result = result;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="podcastName">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="source">Episode download path</param>
        /// <param name="destination">Episode destination/copy path</param>
        public EpisodeDetailEventArgs(string podcastName, string url, string source, string destination)
        {
            PodcastName = podcastName;
            Url = url;
            DownloadPath = source;
            DestinationPath = destination;
        }
    }
}