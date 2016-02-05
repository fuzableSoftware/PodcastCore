namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of subscription completed event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SubscriptionSynchronizedHandler(object sender, SubscriptionTimedEventArgs e);
}