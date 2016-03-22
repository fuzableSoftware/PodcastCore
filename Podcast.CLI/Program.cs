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
        private static void Main()//string[] args)
        {
            ChooseSyncOrCopy();
        }

        private static void ChooseSyncOrCopy()
        {
            Console.WriteLine("1: Synchronize");
            Console.WriteLine("2: Copy 'SELECTIONS' to USB key");
            Console.WriteLine("3: Copy 'HERS' to USB key");
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
                case '3':
                    Console.WriteLine("");
                    CopySubscription("Hers");
                    break;
                default:
                    Console.WriteLine("Unknown option, closing");
                    break;
            }
        }

        private static void SynchronizeSubscription()
        {
            var downloadFolder = Settings.Default.DownloadFolder;
            var subscriptions = new Subscription();

            //attach to sync events
            subscriptions.SubscriptionSynchronizing += Subscription_Synchronizing;
            subscriptions.SubscriptionSynchronized += Subscription_Synchronized;
            subscriptions.PodcastSynchronizing += Podcast_Synchronizing;
            subscriptions.PodcastSynchronized += Podcast_Synchronized;
            subscriptions.EpisodeProcessing += Episode_Processing;
            subscriptions.FolderCreated += Folder_Created;

            //sync
            subscriptions.Synchronize(downloadFolder);

            //return to start
            ChooseSyncOrCopy();
        }

        private static void CopySubscription(string name = "Selections")
        {
            var downloadFolder = Settings.Default.DownloadFolder;
            var subscriptions = new Subscription();

            //attach to copy events
            subscriptions.PodcastCopying += Podcast_Copying;
            subscriptions.PodcastCopied += Podcast_Copied;
            subscriptions.SubscriptionCopying += Subscription_Copying;
            subscriptions.SubscriptionCopied += Subscription_Copied;
            subscriptions.EpisodeProcessing += Episode_Processing;
            subscriptions.FolderCreated += Folder_Created;

            //copy 
            try
            {
                subscriptions.Copy(downloadFolder, Settings.Default.DestinationFolder, name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }

            //return to start
            ChooseSyncOrCopy();
        }

        private static string DurationDescription(TimeSpan duration)
        {
            var desc = "";
            if (duration.TotalMinutes > 1)
            {
                desc = $"{Convert.ToInt32(duration.TotalMinutes)} minutes and ";
            }
            desc += $"{Convert.ToInt32(duration.TotalSeconds)} seconds";
            return desc;
        }

        private static void Subscription_Synchronizing(object sender, SubscriptionEventArgs e)
        {
            Console.WriteLine(e.Count > 1
                ? $"Synchronizing subscription containing {e.Count} podcasts"
                : $"Synchronizing subscription containing {e.Count} podcast");
        }

        private static void Folder_Created(object sender, FolderCreatedEventArgs e)
        {
            Console.WriteLine($"Folder created at {e.Path}");
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
                    Console.WriteLine($"Synchronizing episode '{e.Name}' from {e.Url} to {e.Path}...");
                    break;
                case EpisodeEventArgs.Action.Downloading:
                    Console.WriteLine($"Downloading episode '{e.Name}' from {e.Url} to {e.Path}...");
                    break;
                case EpisodeEventArgs.Action.Deleted:
                    Console.WriteLine($"Episode '{e.Name}' deleted from {e.Path}");
                    break;
                case EpisodeEventArgs.Action.Synchronized:
                    Console.WriteLine(e.Url == null
                        ? $"'{e.Name}' already downloaded"
                        : $"Finished downloading episode '{e.Name}'");
                    break;
                case EpisodeEventArgs.Action.Copying:
                    Console.WriteLine($"File '{e.Name}' is copying to {e.Path}...");
                    break;
                case EpisodeEventArgs.Action.Copied:
                    Console.WriteLine($"File '{e.Name}' successfully copied to {e.Path}");
                    break;
                case EpisodeEventArgs.Action.Updated:
                    Console.WriteLine($"'{e.Path}' already exists");
                    break;
                case EpisodeEventArgs.Action.Error:
                    Console.WriteLine($"'{e.Name}' could not be downloaded from {e.Url} or copied to {e.Path}");
                    break;
            }
        }
        private static void Subscription_Synchronized(object sender, SubscriptionTimedEventArgs e)
        {
            Console.WriteLine($"Subscription with {e.Count} podcasts has finished synchronizing in {DurationDescription(e.Duration)}");
        }
        private static void Subscription_Copied(object sender, SubscriptionTimedEventArgs e)
        {
            Console.WriteLine($"{e.Count} podcasts in subscription have been copied to USB key in {DurationDescription(e.Duration)}");
        }
    }
}
