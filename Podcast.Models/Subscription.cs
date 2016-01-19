using System;
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

        public event SubscriptionOpenedHandler OpenSubscription;

        protected virtual void OnOpenSubscription(int count)
        {
            OpenSubscription?.Invoke(this, new SubscriptionCountEventArgs(count));
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
                OnOpenSubscription(podcasts.Count);
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
        /// Synchronize the subscriopt
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Synchronize(string downloadFolder)
        {
            DownloadFolder = downloadFolder;
            Podcasts = GetPodcasts();
            foreach (var x in Podcasts)
            {
                x.ProcessFeed(downloadFolder);
            }
        }
    }
}
