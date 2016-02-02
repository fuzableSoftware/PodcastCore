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

        #region Constructors

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

        #endregion

        /// <summary>
        /// Returns podcasts in subscription file
        /// </summary>
        /// <returns></returns>
        internal List<Podcast> GetPodcasts()
        {
            //make sure we have somewhere to download to
            try
            {
                VerifyFolderExists(DownloadFolder);
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
                                Order = item.Element("Order")?.Value
                            };
                podcasts.AddRange(items.Select(item => new Podcast(item.Name, item.Url, int.Parse(item.EpisodesToKeep),
                    (Podcast.EpisodeOrder)Enum.Parse(typeof(Podcast.EpisodeOrder), item.Order))));
            }
            catch (Exception ex)
            {
                var error = new ApplicationException("Error retrieving subscriptions", ex);
                throw error;
            }

            return podcasts;
        }

        internal static void VerifyFolderExists(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// Synchronize the subscription
        /// </summary>
        public void Synchronize(string downloadFolder)
        {
            DownloadFolder = downloadFolder;
            Podcasts = GetPodcasts();
            OnSubscriptionSynchronizing(Podcasts.Count);
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
            OnSubscriptionSynchronized(Podcasts.Count);
        }

        /// <summary>
        /// Copy podcasts from download location to USB key
        /// </summary>
        /// <param name="downloadFolder">folder podcasts are downloaded to</param>
        /// <param name="destinationFolder">folder podcasts are copied to</param>
        public void Copy(string downloadFolder, string destinationFolder)
        {
            //does the download folder exist?
            if (!Directory.Exists(downloadFolder))
            {
                throw new FileNotFoundException("Specified download folder does not exist");
            }

            //does the destination folder exist?
            if (!Directory.Exists(destinationFolder))
            {
                throw new FileNotFoundException("Specified destination folder does not exist");
            }

            //get folders in download folder
            var folders = Directory.GetDirectories(downloadFolder);
            var index = 0;
            
            //copy files in each folder to destination
            //if file exists, skip
            foreach (var folder in folders)
            {
                index += 1;
                OnSubscriptionCopying(index, folders.Length);

                var podcastName = folder.Substring(folder.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                OnPodcastCopying(podcastName);

                var files = Directory.GetFiles(folder);

                //reorder files if needed
                if (Podcasts == null || Podcasts.Count == 0)
                {
                    Podcasts = GetPodcasts();
                }
                var podcast = (Podcasts.Find(p => p.Name == podcastName));
                if (podcast?.Order == Podcast.EpisodeOrder.Chronological)
                {
                    //order should be reverse of how they were downloaded
                    //Array.Reverse(files); 
                    //Array.Reverse reorders the array but USB clients (home receiver and car) seem to be listing in alpha (despite car docs indicating write order)
                }

                //count files as they are processed
                var fileIndex = 0;
                foreach (var file in files)
                {
                    //get source path
                    var filename = Path.GetFileName(file) ?? "IDK";
                    //get destination path
                    var podcastFolder = Path.Combine(destinationFolder, podcastName);
                    //destination filename is used by player to organize
                    //default filename is number prefix containing download order
                    //if want downloaded last (first podcast) to be first, need to reverse order here
                    var destination = filename;
                    if (podcast?.Order == Podcast.EpisodeOrder.Chronological)
                    {
                        //reset destination to the reverse number order, same prefix
                        destination = (files.Length - fileIndex).ToString("000") + "_" + filename.Substring(4);
                    }

                    //if the destination has leading zero, trim it
                    if (files.Length < 100)
                    {
                        destination = destination.Substring(1);
                    }

                    //replace underscore with space
                    destination = destination.Replace('_', ' ');

                    //append path to destination
                    destination = Path.Combine(podcastFolder, destination);

                    try
                    {
                        if (!File.Exists(destination))
                        {
                            VerifyFolderExists(podcastFolder);
                            OnEpisodeCopying(podcastName, file, destination);
                            File.Copy(file, destination, false);
                            OnEpisodeCopied(podcastName, file, destination);
                        }
                    }
                    catch (Exception)
                    {
                        OnEpisodeCopyFailed(filename, destination);
#if (DEBUG)
                        {
                            throw;
                        }
#endif
                    }
                    finally
                    {
                        fileIndex += 1;
                    }
                }
                OnPodcastCopied(podcastName);
            }
            OnSubscriptionCopied();
        }

        #region Event Handlers

        private void Episode_EpisodeDownloadFailed(object sender, EpisodeDetailEventArgs eventArgs)
        {
            EpisodeProcessed?.Invoke(sender, new EpisodeDetailEventArgs(eventArgs.Name, eventArgs.Url, eventArgs.DownloadPath, EpisodeDetailEventArgs.EpisodeResult.Failed));
        }

        private void Episode_EpisodeDownloaded(object sender, EpisodeDetailEventArgs eventArgs)
        {
            EpisodeProcessed?.Invoke(sender, new EpisodeDetailEventArgs(eventArgs.Name, eventArgs.Url, eventArgs.DownloadPath, EpisodeDetailEventArgs.EpisodeResult.Downloaded));
        }

        private void Episode_EpisodeDownloading(object sender, EpisodeDetailEventArgs eventArgs)
        {
            EpisodeProcessed?.Invoke(sender, new EpisodeDetailEventArgs(eventArgs.Name, eventArgs.Url, eventArgs.DownloadPath, EpisodeDetailEventArgs.EpisodeResult.Downloading));
        }

        /// <summary>
        /// Event raised when subscription is opened
        /// </summary>
        public event SubscriptionSynchronizingHandler SubscriptionSynchronizing;
        /// <summary>
        /// Handler to raise event when opening subscription
        /// </summary>
        /// <param name="count">Number of podcasts in subscription</param>
        protected virtual void OnSubscriptionSynchronizing(int count)
        {
            SubscriptionSynchronizing?.Invoke(this, new SubscriptionCountEventArgs(count));
        }
        /// <summary>
        /// Event raised when subscription is done
        /// </summary>
        public event SubscriptionSynchronizedHandler SubscriptionSynchronized;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">Number of podcasts in subscription</param>
        protected virtual void OnSubscriptionSynchronized(int count)
        {
            SubscriptionSynchronized?.Invoke(this, new SubscriptionCountEventArgs(count));
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
        /// Event for copying podcast episodes
        /// </summary>
        public event EpisodeCopyingHandler EpisodeCopying;
        /// <summary>
        /// Raises episode copying event
        /// </summary>
        /// <param name="name">Name or title of the episode being copied</param>
        /// <param name="source">Path to the episode being copied</param>
        /// <param name="destination">Destination the episode is being copied to</param>
        protected virtual void OnEpisodeCopying(string name, string source, string destination)
        {
            EpisodeCopying?.Invoke(this, new EpisodeDetailEventArgs(name, null, source, destination));
        }

        /// <summary>
        /// Event indicating episode has been copied
        /// </summary>
        public event EpisodeCopiedHandler EpisodeCopied;
        /// <summary>
        /// Raises episode copied event
        /// </summary>
        /// <param name="name">Title of podcast episode</param>
        /// <param name="source">Path to the episode copied</param>
        /// <param name="destination">Destination the episode was copied to</param>
        protected virtual void OnEpisodeCopied(string name, string source, string destination)
        {
            EpisodeCopied?.Invoke(this, new EpisodeDetailEventArgs(name, null, source, destination));
        }

        /// <summary>
        /// Event indicating episode copy failed
        /// </summary>
        public event EpisodeCopyFailedHandler EpisodeCopyFailed;
        /// <summary>
        /// Raises episode copy failed event
        /// </summary>
        /// <param name="name">Episode title or name</param>
        /// <param name="path">Intended path to episode</param>
        protected virtual void OnEpisodeCopyFailed(string name, string path)
        {
            EpisodeCopyFailed?.Invoke(this, new EpisodeDetailEventArgs(name, path));
        }

        /// <summary>
        /// Event indicates podcast is being copied
        /// </summary>
        public event PodcastCopyingHandler PodcastCopying;
        /// <summary>
        /// Raises podcast copying event
        /// </summary>
        /// <param name="name">Name of the podcast being copied</param>
        protected virtual void OnPodcastCopying(string name)
        {
            PodcastCopying?.Invoke(this, new PodcastDetailEventArgs(name));
        }

        /// <summary>
        /// Event indicating podcast has been copied
        /// </summary>
        public event PodcastCopiedHandler PodcastCopied;
        /// <summary>
        /// Raises the podcast copied event
        /// </summary>
        /// <param name="name"></param>
        protected virtual void OnPodcastCopied(string name)
        {
            PodcastCopied?.Invoke(this, new PodcastDetailEventArgs(name));
        }

        /// <summary>
        /// Event indicating subscription is being copied
        /// </summary>
        public event SubscriptionCopyingHandler SubscriptionCopying;

        /// <summary>
        /// Raises subscription copying event
        /// </summary>
        /// <param name="current">The podcast current being copied</param>
        /// <param name="total">Total number of podcasts to copy</param>
        protected virtual void OnSubscriptionCopying(int current, int total)
        {
            SubscriptionCopying?.Invoke(this, new SubscriptionCountEventArgs(total, current));
        }

        /// <summary>
        /// Event indicating subscription is being copied
        /// </summary>
        public event SubscriptionCopiedHandler SubscriptionCopied;

        /// <summary>
        /// Raises the subscription copied event
        /// </summary>
        protected virtual void OnSubscriptionCopied()
        {
            SubscriptionCopied?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
