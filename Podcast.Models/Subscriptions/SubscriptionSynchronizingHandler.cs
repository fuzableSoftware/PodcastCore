namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of subscription opened event signature
    /// </summary>
    /// <param name="sender">the originator of the event</param>
    /// <param name="eventArgs">Number and current index of podcasts being processed</param>
    public delegate void SubscriptionSynchronizingHandler(object sender, SubscriptionEventArgs eventArgs);
}