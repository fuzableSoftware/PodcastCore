namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Definition of episode synchronized event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event arguments</param>
    public delegate void EpisodeSynchronizedHandler(object sender, EpisodeEventArgs e);
}