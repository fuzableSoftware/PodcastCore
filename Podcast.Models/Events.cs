using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzable.Podcast.Entities
{
    public class SubscriptionCountEventArgs: EventArgs
    {
        public int Count { get; set; }

        public SubscriptionCountEventArgs(int numberOfItems)
        {
            Count = numberOfItems;
        }
    }

    public delegate void SubscriptionOpenedHandler(object sender, SubscriptionCountEventArgs eventArgs);
}
