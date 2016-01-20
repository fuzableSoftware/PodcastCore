namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// Definition of podcast processing event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void PodcastProcessingHandler(object sender, PodcastDetailEventArgs eventArgs);
}