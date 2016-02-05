namespace Fuzable.Podcast.Entities.Episodes
{
    /// <summary>
    /// Definition of episode copying event signature
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event arguments</param>
    public delegate void EpisodeCopyingHandler(object sender, EpisodeCopyEventArgs e);
}