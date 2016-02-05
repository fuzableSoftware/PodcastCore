namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of subscription opened event signature
    /// </summary>
    /// <param name="sender">the originator of the event</param>
    /// <param name="e">Number and current index of podcasts being processed</param>
    public delegate void SubscriptionSynchronizingHandler(object sender, SubscriptionEventArgs e);
}