namespace Fuzable.Podcast.Entities.Podcast
{
    /// <summary>
    /// Definition of podcast opened event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void PodcastOpenedHandler(object sender, PodcastDetailEventArgs eventArgs);
}