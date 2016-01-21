namespace Fuzable.Podcast.Entities.Subscription
{
    /// <summary>
    /// Definition of subscription completed event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void SubscriptionCompletedHandler(object sender, SubscriptionCountEventArgs eventArgs);
}