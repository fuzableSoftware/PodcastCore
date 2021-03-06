namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Definition of episode copy failed event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="eventArgs">event arguments</param>
    public delegate void EpisodeCopyFailedHandler(object sender, EpisodeDetailEventArgs eventArgs);
}