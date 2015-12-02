using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Fuzable.Podcast.Entities;

namespace Podcast.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloadFolder = Properties.Settings.Default.DownloadFolder;
            //read podcasts.xml
            var podcasts = Subscription.GetPodcasts(downloadFolder);
            Console.WriteLine($"Subscribed to {podcasts.Count} podcast(s)");
            //process each returned
            podcasts.ForEach(x => x.ProcessFeed());
            //wait
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
