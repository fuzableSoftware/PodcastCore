namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Definition of episode event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event arguments</param>
    public delegate void EpisodeEventHandler(object sender, EpisodeEventArgs e);
}