using System;

namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of folder creation event signature
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments (default)</param>
    public delegate void FolderCreatedHandler(object sender, FolderCreatedEventArgs e);
}