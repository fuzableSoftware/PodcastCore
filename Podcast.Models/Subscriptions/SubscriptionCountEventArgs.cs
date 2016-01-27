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
        /// <summary>
        /// Current podcast being processed (if applicable)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Constructor with count
        /// </summary>
        /// <param name="numberOfItems">Number of podcasts in subscription</param>
       
        public SubscriptionCountEventArgs(int numberOfItems)
        {
            Count = numberOfItems;
        }

        /// <summary>
        /// Constructor for setting both total and current item
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="currentIndex"></param>
        public SubscriptionCountEventArgs(int totalCount, int currentIndex)
        {
            Count = totalCount;
            Index = currentIndex;
        }
    }
}
