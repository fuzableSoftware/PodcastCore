using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fuzable.Podcast.Models;

namespace Podcast.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var podcasts = Subscription.GetPodcasts();
            podcasts.ForEach(x => x.ProcessFeed());
        }
    }
}
