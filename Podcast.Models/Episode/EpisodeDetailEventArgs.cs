using System;

namespace Fuzable.Podcast.Entities.Episode
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
            /// Episode could not be downloaded
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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        /// <param name="result">Result of download attempt</param>
        public EpisodeDetailEventArgs(string name, string url, string path, EpisodeResult result)
        {
            Name = name;
            Url = url;
            FilePath = path;
            Result = result;
        }
    }
}