using System;

namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of arguments used for folder creation
    /// </summary>
    public class FolderCreatedEventArgs: EventArgs
    {
        /// <summary>
        /// Path to the folder being created
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Constructor with name
        /// </summary>
        /// <param name="path">Path of folder being created</param>
       public FolderCreatedEventArgs(string path)
        {
            Path = path;
        }
    }
}
