using System;

namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Episode detail event arguments
    /// </summary>
    public class EpisodeEventArgs : EventArgs
    {
        /// <summary>
        /// Most recent action for the episode
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// An error occurred, either synchronizing, copying, deleting, etc.
            /// </summary>
            Error,
            /// <summary>
            /// The episode is being synchronized
            /// </summary>
            Synchronizing, 
            /// <summary>
            /// The episode is downloading
            /// </summary>
            Downloading, 
            /// <summary>
            /// The episode has been downloaded
            /// </summary>
            Downloaded, 
            /// <summary>
            /// The episode was downloaded but has been deleted
            /// </summary>
            Deleted, 
            /// <summary>
            /// The episode has completed synchronization
            /// </summary>
            Synchronized,
            /// <summary>
            /// The episode is being copied to the USB key
            /// </summary>
            Copying, 
            /// <summary>
            /// The episode has been copied to the USB key
            /// </summary>
            Copied
        }

        /// <summary>
        /// Action for this episode
        /// </summary>
        public Action Activity { get; set; }

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
        /// <param name="activity">Most recent Action for this episode</param>
        /// <param name="name">Episode name</param>
        public EpisodeEventArgs(Action activity, string name)
        {
            Name = name;
        }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="activity">Most recent Action for this episode</param>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        public EpisodeEventArgs(Action activity, string name, string url)
        {
            Name = name;
            Url = url;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activity">Most recent Action for this episode</param>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        public EpisodeEventArgs(Action activity, string name, string url, string path)
        {
            Name = name;
            Url = url;
            Path = path;
        }
    }
}