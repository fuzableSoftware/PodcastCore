namespace Fuzable.Podcast.Entities.Podcasts
{
    /// <summary>
    /// Definition of podcast opened event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PodcastSynchronizingHandler(object sender, PodcastDetailEventArgs e);
}