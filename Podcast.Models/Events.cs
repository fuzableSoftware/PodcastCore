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

    public class PodcastDetailEventArgs : EventArgs
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int EpisodesToDownload { get; set; }
        public int EpisodesToDelete { get; set; }

        public PodcastDetailEventArgs(string name)
        {
            Name = name;
        }

        public PodcastDetailEventArgs(string name, string url, int episodesToDownload, int episodesToDelete)
        {
            Name = name;
            Url = url;
            EpisodesToDownload = episodesToDownload;
            EpisodesToDelete = EpisodesToDelete;
        }
    }

    public delegate void SubscriptionOpenedHandler(object sender, SubscriptionCountEventArgs eventArgs);
    public delegate void PodcastOpenedHandler(object sender, PodcastDetailEventArgs eventArgs);
    public delegate void PodcastProcessingHandler(object sender, PodcastDetailEventArgs eventArgs);
    public delegate void SubscriptionCompletedHandler(object sender, SubscriptionCountEventArgs eventArgs);
}
