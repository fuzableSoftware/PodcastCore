namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Definition of episode downloading event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event arguments</param>
    public delegate void EpisodeDownloadingHandler(object sender, EpisodeEventArgs e);
}