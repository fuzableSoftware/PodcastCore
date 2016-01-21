using System;

namespace Fuzable.Podcast.Entities
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
        /// Constructor with count
        /// </summary>
        /// <param name="numberOfItems">Number of podcasts in subscription</param>
       
        public SubscriptionCountEventArgs(int numberOfItems)
        {
            Count = numberOfItems;
        }
    }
}
