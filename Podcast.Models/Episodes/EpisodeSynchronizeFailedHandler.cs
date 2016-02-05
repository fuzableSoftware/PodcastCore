namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Definition of episode downloaded event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event arguments</param>
    public delegate void EpisodeSynchronizeFailedHandler(object sender, EpisodeEventArgs e);
}