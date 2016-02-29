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
            subscriptions.EpisodeProcessing += Episode_Processing;
            subscriptions.FolderCreated += FolderCreated;

            //sync
            subscriptions.Synchronize(downloadFolder);
        }

        private static void CopySubscription()
        {
            var downloadFolder = Settings.Default.DownloadFolder;
            var subscriptions = new Subscription("podcast.xml");

            //attach to copy events
            subscriptions.PodcastCopying += Podcast_Copying;
            subscriptions.PodcastCopied += Podcast_Copied;
            subscriptions.SubscriptionCopying += Subscription_Copying;
            subscriptions.SubscriptionCopied += Subscription_Copied;
            subscriptions.EpisodeProcessing += Episode_Processing;
            subscriptions.FolderCreated += FolderCreated;

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

        private static void Subscription_Copying(object sender, SubscriptionEventArgs e)
        {
            Console.WriteLine($"Subscription copying to USB key, copying podcast {e.Index} of {e.Count}...");
        }

        private static void Podcast_Copying(object sender, PodcastDetailEventArgs e)
        {
            Console.WriteLine($"Podcast '{e.Name}' is being copied to USB key...");
        }
        private static void Podcast_Copied(object sender, PodcastDetailEventArgs e)
        {
            Console.WriteLine($"Podcast '{e.Name}' has been copied to USB key");
        }

        private static void Episode_Processing(object sender, EpisodeEventArgs e)
        {
            switch (e.Activity)
            {
                case EpisodeEventArgs.Action.Synchronizing:
                    Console.WriteLine($"Downloading episode '{e.Name}' from {e.Url} to {e.Path}...");
                    break;
                case EpisodeEventArgs.Action.Deleted:
                    Console.WriteLine($"Episode '{e.Name}' deleted from {e.Path}...");
                    break;
                case EpisodeEventArgs.Action.Synchronized:
                    Console.WriteLine(e.Url == null
                        ? $"'{e.Name}' already downloaded"
                        : $"Finished downloading episode '{e.Name}'");
                    break;
                case EpisodeEventArgs.Action.Copying:
                    Console.WriteLine(e.Name == null
                                    ? $"File already exists at {e.Path}"
                                    : $"File {e.Path} successfully copied to {e.Path}");
                    break;
                case EpisodeEventArgs.Action.Copied:
                    Console.WriteLine(e.Name == null
                                    ? $"File already exists at {e.Path}"
                                    : $"File {e.Path} successfully copied to {e.Path}");
                    break;
                case EpisodeEventArgs.Action.Error:
                    Console.WriteLine($"'{e.Name}' could not be downloaded from {e.Url} or copied to {e.Path}");
                    break;
            }
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
        private static void Subscription_Copied(object sender, SubscriptionTimedEventArgs e)
        {
            Console.WriteLine($"{e.Count} podcasts in subscription have been copied to USB key in {e.Duration.Seconds} seconds");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
