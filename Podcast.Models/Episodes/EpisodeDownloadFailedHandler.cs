namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Definition of episode download failed event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="eventArgs">event arguments</param>
    public delegate void EpisodeDownloadFailedHandler(object sender, EpisodeDownloadingEventArgs eventArgs);
}