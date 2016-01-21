namespace Fuzable.Podcast.Entities.Subscription
{
    /// <summary>
    /// Definition of subscription opened event signature
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>

    public delegate void SubscriptionOpenedHandler(object sender, SubscriptionCountEventArgs eventArgs);
}