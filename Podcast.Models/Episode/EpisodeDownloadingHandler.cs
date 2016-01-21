namespace Fuzable.Podcast.Entities.Episode
{
    /// <summary>
    /// Definition of episode downloading event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="eventArgs">event arguments</param>
    public delegate void EpisodeDownloadingHandler(object sender, EpisodeDetailEventArgs eventArgs);
}