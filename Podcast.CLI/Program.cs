using System;
using Fuzable.Podcast.Entities;
using Fuzable.Podcast.Entities.Episodes;
using Fuzable.Podcast.Entities.Podcasts;
using Fuzable.Podcast.Entities.Subscriptions;
using Podcast.CLI.Properties;

namespace Podcast.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloadFolder = Settings.Default.DestinationFolder;
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
            Console.WriteLine($"Retrieved information from {eventArgs.Url} for {eventArgs.Name} will download {eventArgs.EpisodesToDownload} and delete up to {eventArgs.EpisodesToDelete} episodes");
        }

        private static void Episode_Processed(object sender, EpisodeDetailEventArgs eventArgs)
        {
            switch (eventArgs.Result)
            {
                case EpisodeDetailEventArgs.EpisodeResult.Downloading:
                    Console.WriteLine($"Downloading episode {eventArgs.Name} from {eventArgs.Url} to {eventArgs.FilePath}...");
                    break;
                case EpisodeDetailEventArgs.EpisodeResult.Downloaded:
                    Console.WriteLine($"Downloaded episode {eventArgs.Name} to {eventArgs.FilePath}");
                    break;
                case EpisodeDetailEventArgs.EpisodeResult.Failed:
                    Console.WriteLine($"FAILED downloading episode {eventArgs.Name} from {eventArgs.Url} to {eventArgs.FilePath}");
                    break;
            }
        }

        static void Subscription_Completed(object sender, SubscriptionCountEventArgs eventArgs)
        {
            Console.WriteLine($"Subscription with {eventArgs.Count} podcast(s) has finished processing");
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
