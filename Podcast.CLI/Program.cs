using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            var subscriptions = new Subscription("podcast.xml");
            subscriptions.Sync(downloadFolder);
            //Console.WriteLine($"Subscribed to {podcasts.Count} podcast(s)");
            //process each returned
            //foreach (var x in podcasts)
            //{
            //   //Console.WriteLine($"Processing {x.Name}...");
            //   x.ProcessFeed(downloadFolder);
            //   //Console.WriteLine($"Retrieved information from {x.Url}, will download {x.EpisodesToDownload.Count} and delete up to {x.EpisodesToDelete.Count} episodes");
            //}
            //wait
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
