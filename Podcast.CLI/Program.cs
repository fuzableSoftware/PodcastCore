using System;
using Fuzable.Podcast.Entities;
using Fuzable.Podcast.Entities.Episodes;
using Fuzable.Podcast.Entities.Podcasts;
using Fuzable.Podcast.Entities.Subscriptions;
using Podcast.CLI.Properties;

namespace Podcast.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ChooseSyncOrCopy();
        }

        private static void ChooseSyncOrCopy()
        {
            Console.WriteLine("1: Synchronize");
            Console.WriteLine("2: Copy to USB key");
            var x = Console.ReadKey();
            switch (x.KeyChar)
            {
                case '1':
                    Console.WriteLine("");
                    SynchronizeSubscription();
                    break;
                case '2':
                    Console.WriteLine("");
                    CopySubscription();
                    break;
                default:
                    Console.WriteLine("Unknown option, closing");
                    break;
            }
        }

        private static void SynchronizeSubscription()
        {
            var downloadFolder = Settings.Default.DownloadFolder;
            var subscriptions = new Subscription("podcast.xml");

            //attach to sync events
            subscriptions.SubscriptionSynchronizing += SubscriptionSynchronizing;
            subscriptions.SubscriptionSynchronized += SubscriptionSynchronized;
            subscriptions.PodcastSynchronizing += Podcast_Synchronizing;
            subscriptions.PodcastSynchronized += Podcast_Synchronized;
            subscriptions.EpisodeSynchronizing += EpisodeSynchronizing;
            subscriptions.EpisodeSynchronized += EpisodeSynchronized;
            subscriptions.EpisodeSynchronizeFailed += EpisodeSynchronizeFailed;
            subscriptions.FolderCreated += FolderCreated;

            //sync
            subscriptions.Synchronize(downloadFolder);
        }

        private static void CopySubscription()
        {
            var downloadFolder = Settings.Default.DownloadFolder;
            var subscriptions = new Subscription("podcast.xml");

            //attach to copy events
            subscriptions.EpisodeProcessing += Episode_Processing;
            subscriptions.EpisodeCopying += Episode_Copying;
            subscriptions.EpisodeCopyFailed += Episode_CopyFailed;
            subscriptions.PodcastCopying += Podcast_Copying;
            subscriptions.PodcastCopied += Podcast_Copied;
            subscriptions.SubscriptionCopying += Subscription_Copying;
            subscriptions.SubscriptionCopied += Subscription_Copied;

            //copy 
            try
            {
                subscriptions.Copy(downloadFolder, Settings.Default.DestinationFolder, "Selections");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }

        private static void FolderCreated(object sender, FolderCreatedEventArgs e)
        {
            Console.WriteLine($"Folder created at {e.Path}");
        }

        private static void SubscriptionSynchronizing(object sender, SubscriptionEventArgs e)
        {
            Console.WriteLine(e.Count > 1
                ? $"Synchronizing subscription containing {e.Count} podcasts"
                : $"Synchronizing subscription containing {e.Count} podcast");
        }

        private static void Podcast_Synchronizing(object sender, PodcastDetailEventArgs e)
        {
            if (e.EpisodesToDownload == -1)
            {
                //unknown, feed not opened
                Console.WriteLine($"Processing podcast feed for '{e.Name}'");
            }
            else
            {
                Console.WriteLine($"Synchronizing podcast '{e.Name}' at {e.Url}...");
                Console.WriteLine($"** {e.Name} may download {e.EpisodesToDownload} and remove up to {e.EpisodesToDelete} episodes **");
            }
        }

        private static void Podcast_Synchronized(object sender, PodcastDetailEventArgs e)
        {
            Console.WriteLine($"Finished synchronizing '{e.Name}'");
        }

        private static void EpisodeSynchronizing(object sender, EpisodeEventArgs e)
        {
            Console.WriteLine($"Downloading episode '{e.Name}' from {e.Url} to {e.Path}...");
        }

        private static void EpisodeSynchronized(object sender, EpisodeEventArgs e)
        {
            if (e.Url == null)
            {
                Console.WriteLine($"'{e.Name}' already downloaded");
            }
            else
            {
                Console.WriteLine($"Finished downloading episode '{e.Name}'");
            }
        }

        private static void EpisodeSynchronizeFailed(object sender, EpisodeEventArgs e)
        {
            Console.WriteLine($"Failed downloading episode '{e.Name}' from {e.Url}");
        }

        private static void SubscriptionSynchronized(object sender, SubscriptionTimedEventArgs e)
        {
            Console.WriteLine($"Subscription with {e.Count} podcast(s) has finished synchronizing in {e.Duration.Seconds} seconds");
            Console.WriteLine("Press X to abort or press any key to copy downloaded podcasts to USB key...");
            var answer = Console.ReadKey();
            if (answer.KeyChar == 'X')
            {
                Environment.Exit(0);
            }
        }

        private static void Subscription_Copying(object sender, SubscriptionEventArgs e)
        {
            Console.WriteLine($"Subscription copying to USB key, copying podcast {e.Index} of {e.Count}...");
        }

        private static void Subscription_Copied(object sender, SubscriptionTimedEventArgs e)
        {
            Console.WriteLine($"{e.Count} podcasts in subscription have been copied to USB key in {e.Duration.Seconds} seconds");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void Podcast_Copied(object sender, PodcastDetailEventArgs e)
        {
            Console.WriteLine($"Podcast '{e.Name}' has been copied to USB key");
        }

        private static void Podcast_Copying(object sender, PodcastDetailEventArgs e)
        {
            Console.WriteLine($"Podcast '{e.Name}' is being copied to USB key...");
        }

        private static void Episode_Copying(object sender, EpisodeEventArgs e)
        {
            Console.WriteLine(e.Name == null
                ? $"File already exists at {e.Path}"
                : $"Copying {e.Name} to {e.Path}...");
        }
        private static void Episode_Copied(object sender, EpisodeEventArgs e)
        {
            Console.WriteLine(e.Name == null
                ? $"File already exists at {e.Path}"
                : $"File {e.Path} successfully copied to {e.Path}");
        }

        private static void Episode_CopyFailed(object sender, EpisodeEventArgs e)
        {
            Console.WriteLine($"'{e.Name}' could not be copied to {e.Path}!");
        }

        private static void Episode_Processing(object sender, EpisodeEventArgs e)
        {
            switch (e.Activity)
            {
                case EpisodeEventArgs.Action.Copying:
                    Console.WriteLine(e.Name == null
                                    ? $"File already exists at {e.Path}"
                                    : $"File {e.Path} successfully copied to {e.Path}");
                    break;
            }
        }
    }
}
