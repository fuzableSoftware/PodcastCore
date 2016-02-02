﻿using System;
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
            var downloadFolder = Settings.Default.DownloadFolder;
            //read podcasts.xml
            var subscriptions = new Subscription("podcast.xml");

            //attach to sync events
            subscriptions.SubscriptionSynchronizing += SubscriptionSynchronizing;
            subscriptions.SubscriptionSynchronized += SubscriptionSynchronized;
            subscriptions.PodcastSynchronizing += Podcast_Synchronizing;
            subscriptions.PodcastSynchronized += Podcast_Synchronized;
            subscriptions.EpisodeSynchronizing += EpisodeSynchronizing;
            subscriptions.EpisodeSynchronized += EpisodeSynchronized;

            //sync
            subscriptions.Synchronize(downloadFolder);

            //atach to copy events
            subscriptions.EpisodeCopying += Episode_Copying;
            subscriptions.EpisodeCopied += Episode_Copied;
            subscriptions.EpisodeCopyFailed += Episode_CopyFailed;
            subscriptions.PodcastCopying += Podcast_Copying;
            subscriptions.PodcastCopied += Podcast_Copied;
            subscriptions.SubscriptionCopying += Subscription_Copying;
            subscriptions.SubscriptionCopied += Subscription_Copied;

            //copy 
            subscriptions.Copy(downloadFolder, Settings.Default.DestinationFolder);
        }

        private static void SubscriptionSynchronizing(object sender, SubscriptionCountEventArgs eventArgs)
        {
            Console.WriteLine($"Syncing subscription containing {eventArgs.Count} podcast(s)...");
        }

        private static void Podcast_Synchronizing(object sender, PodcastDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Synchronizing podcast '{eventArgs.Name}' at {eventArgs.Url}...");
            Console.WriteLine($"Synchronizing {eventArgs.Name} may download {eventArgs.EpisodesToDownload} and remove up to {eventArgs.EpisodesToDelete} episodes...");
        }
        
        private static void Podcast_Synchronized(object sender, PodcastDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Finished synchronizing '{eventArgs.Name}'");
        }

        private static void EpisodeSynchronizing(object sender, EpisodeDetailEventArgs eventArgs)
        {
            switch (eventArgs.Result)
            {
                case EpisodeDetailEventArgs.EpisodeResult.Downloading:
                    Console.WriteLine(
                        $"Downloading episode '{eventArgs.Name}' from {eventArgs.Url} to {eventArgs.DownloadPath}...");
                    break;
                case EpisodeDetailEventArgs.EpisodeResult.Downloaded:
                    Console.WriteLine($"Downloaded episode '{eventArgs.Name}' to {eventArgs.DownloadPath}");
                    break;
                case EpisodeDetailEventArgs.EpisodeResult.Failed:
                    Console.WriteLine(
                        $"FAILED downloading episode '{eventArgs.Name}' from {eventArgs.Url} to {eventArgs.DownloadPath}");
                    break;
            }
        }

        private static void EpisodeSynchronized(object sender, EpisodeDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Finished processing episode '{eventArgs.Name}'");
        }

        private static void SubscriptionSynchronized(object sender, SubscriptionCountEventArgs eventArgs)
        {
            Console.WriteLine($"Subscription with {eventArgs.Count} podcast(s) has finished synchronizing");
            Console.WriteLine("Press any key to copy downloaded podcasts to USB key...");
            Console.ReadKey();
        }

        private static void Subscription_Copying(object sender, SubscriptionCountEventArgs e)
        {
            Console.WriteLine($"Subscription copying to USB key, copying podcast {e.Index} of {e.Count}...");
        }

        private static void Subscription_Copied(object sender, EventArgs e)
        {
            Console.WriteLine("Podcasts in subscription have been copied to USB key");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void Podcast_Copied(object sender, PodcastDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Podcast '{eventArgs.Name}' has been copied to USB key");
        }

        private static void Podcast_Copying(object sender, PodcastDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Podcast '{eventArgs.Name}' is being copied to USB key...");
        }

        private static void Episode_Copying(object sender, EpisodeDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Copying episode '{eventArgs.Name}' from {eventArgs.DownloadPath} to {eventArgs.DestinationPath}...");
        }
        private static void Episode_Copied(object sender, EpisodeDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Episode '{eventArgs.Name}' was successfully copied from {eventArgs.DownloadPath} to {eventArgs.DestinationPath}");
        }
        
        private static void Episode_CopyFailed(object sender, EpisodeDetailEventArgs eventArgs)
        {
            Console.WriteLine($"Episode '{eventArgs.Name}' could not be copied from {eventArgs.DownloadPath} to {eventArgs.DestinationPath}!");
        }

    }
}
