namespace Fuzable.Podcast.Entities.Podcasts
{
    /// <summary>
    /// Definition of podcast copying event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void PodcastCopyingHandler(object sender, PodcastDetailEventArgs eventArgs);
}