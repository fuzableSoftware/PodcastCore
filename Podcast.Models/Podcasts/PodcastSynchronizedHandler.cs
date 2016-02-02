namespace Fuzable.Podcast.Entities.Podcasts
{
    /// <summary>
    /// Definition of podcast opened event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void PodcastSynchronizedHandler(object sender, PodcastDetailEventArgs eventArgs);
}