namespace Fuzable.Podcast.Entities.Episode
{
    /// <summary>
    /// Definition of episode downloaded event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="eventArgs">event arguments</param>
    public delegate void EpisodeDownloadedHandler(object sender, EpisodeDetailEventArgs eventArgs);
}