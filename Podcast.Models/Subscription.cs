using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Fuzable.Podcast.Entities.Episodes;
using Fuzable.Podcast.Entities.Podcasts;
using Fuzable.Podcast.Entities.Subscriptions;

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

        /// <summary>
        /// Event raised when subscription is opened
        /// </summary>
        public event SubscriptionOpenedHandler SubscriptionOpened;
        /// <summary>
        /// Handler to raise event when opening subscription
        /// </summary>
        /// <param name="count">Number of podcasts in subscription</param>
        protected virtual void OnSubscriptionOpened(int count)
        {
            SubscriptionOpened?.Invoke(this, new SubscriptionCountEventArgs(count));
        }
        /// <summary>
        /// Event raised when subscription is done
        /// </summary>
        public event SubscriptionCompletedHandler SubscriptionCompleted;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">Number of podcasts in subscription</param>
        protected virtual void OnSubscriptionCompleted(int count)
        {
            SubscriptionCompleted?.Invoke(this, new SubscriptionCountEventArgs(count));
        }

        /// <summary>
        /// Event raised when podcast is opened
        /// </summary>
        public event PodcastOpenedHandler PodcastOpened;
        /// <summary>
        /// Handler to raise event when opening podcast
        /// </summary>
        /// <param name="name">Podcast name</param>
        protected virtual void OnPodcastOpened(string name)
        {
            PodcastOpened?.Invoke(this, new PodcastDetailEventArgs(name));
        }

        /// <summary>
        /// Event raised when podcast is processed
        /// </summary>
        public event PodcastProcessingHandler PodcastProcessed;
        /// <summary>
        /// Handler to raise event when processing podcast
        /// </summary>
        /// <param name="name">Name of podcast</param>
        /// <param name="url">Podcast address</param>
        /// <param name="episodesToDownload">Number of episodes to download</param>
        /// <param name="episodesToDelete">Number of episodes to delete</param>
        protected virtual void OnPodcastProcessed(string name, string url, int episodesToDownload, int episodesToDelete)
        {
            PodcastProcessed?.Invoke(this, new PodcastDetailEventArgs(name, url, episodesToDownload, episodesToDelete));
        }

        /// <summary>
        /// Episode processed event
        /// </summary>
        public event EpisodeProcessedHandler EpisodeProcessed;
        /// <summary>
        /// Raises episode processed event with result
        /// </summary>
        /// <param name="name">Episode name</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        /// <param name="result">Success result of episode</param>
        protected virtual void OnEpisodeProcessed(string name, string url, string path, EpisodeDetailEventArgs.EpisodeResult result)
        {
            EpisodeProcessed?.Invoke(this, new EpisodeDetailEventArgs(name, url, path, result));
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

        internal static void VerifyDownloadFolderExists(string downloadFolder)
        {
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }
        }

        /// <summary>
        /// Synchronize the subscription
        /// </summary>
        public void Synchronize(string downloadFolder)
        {
            DownloadFolder = downloadFolder;
            Podcasts = GetPodcasts();
            OnSubscriptionOpened(Podcasts.Count);
            foreach (var podcast in Podcasts)
            {
                OnPodcastOpened(podcast.Name);
                podcast.ProcessFeed(downloadFolder);
                OnPodcastProcessed(podcast.Name, podcast.Url, podcast.EpisodesToDownload.Count, podcast.EpisodesToDelete.Count);
                foreach (var episode in podcast.EpisodesToDownload)
                {
                    episode.EpisodeDownloading += Episode_EpisodeDownloading;
                    episode.EpisodeDownloaded += Episode_EpisodeDownloaded;
                    episode.EpisodeDownloadFailed += Episode_EpisodeDownloadFailed;
                    episode.Download();
                }
            }
            OnSubscriptionCompleted(Podcasts.Count);
        }

        private void Episode_EpisodeDownloadFailed(object sender, EpisodeDetailEventArgs eventArgs)
        {
            EpisodeProcessed?.Invoke(sender, new EpisodeDetailEventArgs(eventArgs.Name, eventArgs.Url, eventArgs.FilePath, EpisodeDetailEventArgs.EpisodeResult.Failed));
        }

        private void Episode_EpisodeDownloaded(object sender, EpisodeDetailEventArgs eventArgs)
        {
            EpisodeProcessed?.Invoke(sender, new EpisodeDetailEventArgs(eventArgs.Name, eventArgs.Url, eventArgs.FilePath, EpisodeDetailEventArgs.EpisodeResult.Downloaded));
        }

        private void Episode_EpisodeDownloading(object sender, EpisodeDetailEventArgs eventArgs)
        {
            EpisodeProcessed?.Invoke(sender, new EpisodeDetailEventArgs(eventArgs.Name, eventArgs.Url, eventArgs.FilePath, EpisodeDetailEventArgs.EpisodeResult.Downloading));
        }
    }
}
