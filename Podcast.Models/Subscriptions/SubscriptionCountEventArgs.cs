using System;

namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of arguments used for subscription count
    /// </summary>
    public class SubscriptionCountEventArgs: EventArgs
    {
        /// <summary>
        /// Number of podcasts in subscription
        /// </summary>
        public int Count { get; set; }
        public int Index { get; set; }

        /// <summary>
        /// Constructor with count
        /// </summary>
        /// <param name="numberOfItems">Number of podcasts in subscription</param>
       
        public SubscriptionCountEventArgs(int numberOfItems)
        {
            Count = numberOfItems;
        }
        public SubscriptionCountEventArgs(int totalCount, int currentIndex)
        {
            Count = totalCount;
            Index = currentIndex;
        }
    }
}
