﻿using System;

namespace Fuzable.Podcast.Entities.Subscriptions
{
    /// <summary>
    /// Definition of arguments used for subscription count
    /// </summary>
    public class SubscriptionEventArgs: EventArgs
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
        /// The time elapsed for this activity (if applicable)
        /// </summary>
        public TimeSpan Duration { get; set; } 
        
        /// <summary>
        /// Constructor for setting both total and current item
        /// </summary>
        /// <param name="totalCount">Number of items</param>
        /// <param name="currentIndex">Current item</param>
        public SubscriptionEventArgs(int totalCount, int currentIndex)
        {
            Count = totalCount;
            Index = currentIndex;
        }

        /// <summary>
        /// Constructor for setting both total and duration
        /// </summary>
        /// <param name="totalCount">Number of items</param>
        /// <param name="duration">How long the operation took</param>
        public SubscriptionEventArgs(int totalCount, TimeSpan duration)
        {
            Count = totalCount;
            Duration = duration;
        }
    }
}