using System;

namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of subscription copying event signature
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments (default)</param>
    public delegate void SubscriptionCopyingHandler(object sender, EventArgs e);
}