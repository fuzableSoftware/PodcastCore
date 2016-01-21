using System;
using Fuzable.Podcast.Entities.Episode;
using Fuzable.Podcast.Entities.Podcast;
using Fuzable.Podcast.Entities.Subscription;
using Podcast.CLI.Properties;

namespace Podcast.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloadFolder = Settings.Default.DownloadFolder;
            //read podcasts.xml
            var subscriptions = new Subscription("podcast.xml");
            //attach to events
            subscriptions.SubscriptionOpened += Subscription_Opened;
            subscriptions.SubscriptionCompleted += Subscription_Completed;
            subscriptions.PodcastOpened += Podcast_Opened;
            subscriptions.PodcastProcessed += Podcast_Processed;
            subscriptions.EpisodeProcessed += Episode_Processed;
            //sync
            subscriptions.Synchronize(downloadFolder);
        }

        private static void Episode_Processed(object sender, EpisodeDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Processed episode {eventArgs.Name} at {eventArgs.Url}");
        }

        static void Subscription_Opened(object sender, SubscriptionCountEventArgs eventArgs)
        {
            Console.WriteLine($"Subscribed to {eventArgs.Count} podcast(s)");
        }

        static void Podcast_Opened(object sender, PodcastDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Opening podcast '{eventArgs.Name}'");
        }

        static void Podcast_Processed(object sender, PodcastDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Retrieved information from {eventArgs.Url}");
            Console.WriteLine($"for {eventArgs.Name} will download {eventArgs.EpisodesToDownload} and delete up to {eventArgs.EpisodesToDelete} episodes");
        }

        static void Subscription_Completed(object sender, SubscriptionCountEventArgs eventArgs)
        {
            Console.WriteLine($"Subscription with {eventArgs.Count} podcast(s) has finished processing");
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
