﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Xml.Linq;

namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// Manages subscription file (podcasts.xml)
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Podcasts in the subscription
        /// </summary>
        public List<Podcast> Podcasts { get; set; }

        /// <summary>
        /// Filename used to manage subscriptions
        /// </summary>
        public string SubscriptionFile { get; set; }

        /// <summary>
        /// Folder to sync podcasts to
        /// </summary>
        public string DownloadFolder { get; set; }

        public event SubscriptionOpenedHandler SubscriptionOpened;
        protected virtual void OnSubscriptionOpened(int count)
        {
            SubscriptionOpened?.Invoke(this, new SubscriptionCountEventArgs(count));
        }
        public event SubscriptionCompletedHandler SubscriptionCompleted;
        protected virtual void OnSubscriptionCompleted(int count)
        {
            SubscriptionCompleted?.Invoke(this, new SubscriptionCountEventArgs(count));
        }

        public event PodcastOpenedHandler PodcastOpened;
        protected virtual void OnPodcastOpened(string name)
        {
            PodcastOpened?.Invoke(this, new PodcastDetailEventArgs(name));
        }

        public event PodcastProcessingHandler PodcastProcessed;
        protected virtual void OnPodcastProcessed(string name, string url, int episodesToKeep, int episodesToDelete)
        {
            PodcastProcessed?.Invoke(this, new PodcastDetailEventArgs(name, url, episodesToKeep, episodesToDelete));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Subscription()
        {
            SubscriptionFile = "podcasts/xml";
        }
        
        /// <summary>
        /// Constructor specifying subscription file
        /// </summary>
        /// <param name="subscriptionFile"></param>
        public Subscription(string subscriptionFile)
        {
            SubscriptionFile = subscriptionFile;
        }

        /// <summary>
        /// Returns podcasts in subscription file
        /// </summary>
        /// <returns></returns>
        internal List<Podcast> GetPodcasts()
        {
            //make sure we have somewhere to download to
            try
            {
                VerifyDownloadFolderExists(DownloadFolder);
            }
            catch (Exception ex)
            {
                var error = new ApplicationException($"Error locating download folder '{DownloadFolder}'", ex);
                throw error;
            }
            
            //read podcasts.xml file and extract subscribed podcasts from and return
            var podcasts = new List<Podcast>();

            try
            {
                var settingsDoc = XDocument.Load($@"{Environment.CurrentDirectory}\{"Podcasts.xml"}");

                var items = from item in settingsDoc.Descendants("Podcast")
                            select new
                            {
                                Name = item.Element("Name")?.Value,
                                Url = item.Element("Url")?.Value,
                                EpisodesToKeep = item.Element("EpisodesToKeep")?.Value,
                            };
                podcasts.AddRange(items.Select(item => new Podcast(item.Name, item.Url, int.Parse(item.EpisodesToKeep))));
            }
            catch (Exception ex)
            {
                var error = new ApplicationException("Error retrieving subscriptions", ex);
                throw error;
            }

            return podcasts;
        }

        private static void VerifyDownloadFolderExists(string downloadFolder)
        {
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }
        }

        /// <summary>
        /// Synchronize the subscription
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Synchronize(string downloadFolder)
        {
            DownloadFolder = downloadFolder;
            Podcasts = GetPodcasts();
            OnSubscriptionOpened(Podcasts.Count);
            foreach (var x in Podcasts)
            {
                OnPodcastOpened(x.Name);
                x.ProcessFeed(downloadFolder);
                OnPodcastProcessed(x.Name, x.Url, x.EpisodesToKeep, x.EpisodesToDelete.Count);
            }
            OnSubscriptionCompleted(Podcasts.Count);
        }
    }
}
