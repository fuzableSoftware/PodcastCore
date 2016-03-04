﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Fuzable.Podcast.Entities.Episodes;
using Fuzable.Podcast.Entities.Podcasts;
using Fuzable.Podcast.Entities.Properties;
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

        #region Events

        /// <summary>
        /// Event raised when a folder is being created
        /// </summary>
        public event FolderCreatedHandler FolderCreated;

        /// <summary>
        /// Event raised when subscription is synchronizing
        /// </summary>
        public event SubscriptionSynchronizingHandler SubscriptionSynchronizing;

        /// <summary>
        /// Event raised when subscription is done synchronizing
        /// </summary>
        public event SubscriptionSynchronizedHandler SubscriptionSynchronized;

        /// <summary>
        /// Event indicating subscription is being copied
        /// </summary>
        public event SubscriptionCopyingHandler SubscriptionCopying;

        /// <summary>
        /// Event indicating subscription is being copied
        /// </summary>
        public event SubscriptionCopiedHandler SubscriptionCopied;

        /// <summary>
        /// Event raised when podcast is synchronizing
        /// </summary>
        public event PodcastSynchronizingHandler PodcastSynchronizing;

        /// <summary>
        ///  Event raised when podcast is synchronized
        /// </summary>
        public event PodcastSynchronizedHandler PodcastSynchronized;

        /// <summary>
        /// Event indicates podcast is being copied
        /// </summary>
        /// 
        public event PodcastCopyingHandler PodcastCopying;

        /// <summary>
        /// Event indicating podcast has been copied
        /// </summary>
        public event PodcastCopiedHandler PodcastCopied;

        /// <summary>
        /// Episode processing event
        /// </summary>
        public event EpisodeEventHandler EpisodeProcessing;
      
        #endregion

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
                var items = from item in settingsDoc.Root?.Elements("Podcasts").Descendants("Podcast")
                            select new
                            {
                                Name = item.Element("Name")?.Value,
                                Url = item.Element("Url")?.Value,
                                Download = item.Element("Download")?.Value,
                                Order = item.Element("Order")?.Value, 
                                RemoveFromTitle = item.Element("RemoveFromTitle")?.Value, 
                                ExcludeTitle = item.Element("ExcludeTitle")?.Value
                            };
                podcasts.AddRange(items.Select(item => new Podcast(item.Name, item.Url, int.Parse(item.Download),
                    (Podcast.EpisodeOrder)Enum.Parse(typeof(Podcast.EpisodeOrder), item.Order), item.RemoveFromTitle, item.ExcludeTitle)));
            }
            catch (Exception ex)
            {
                var error = new ApplicationException("Error retrieving subscriptions", ex);
                throw error;
            }

            return podcasts;
        }

        internal void VerifyFolderExists(string folder)
        {
            if (Directory.Exists(folder)) return;
            Directory.CreateDirectory(folder);
            OnFolderCreated(folder);
        }

        /// <summary>
        /// Synchronize the subscription
        /// </summary>
        public void Synchronize(string downloadFolder)
        {
            var start = DateTime.Now;
            DownloadFolder = downloadFolder;
            Podcasts = GetPodcasts();
            OnSubscriptionSynchronizing(Podcasts.Count);
            foreach (var podcast in Podcasts)
            {
                //raise this without info, before processing feed (since is complex operation, could fail)
                OnPodcastSynchronizing(podcast.Name);
                podcast.ProcessFeed(this);
                OnPodcastSynchronizing(podcast.Name, podcast.Url, podcast.EpisodesToDownload.Count, podcast.EpisodesToDelete.Count);

               //process each episode to download
                foreach (var episode in podcast.EpisodesToDownload)
                {
                    episode.EpisodeProcessing += Episode_EpisodeProcessing;
                    episode.Download();
                }

                //process each episode to delete
                foreach (var episode in podcast.EpisodesToDelete)
                {
                    episode.EpisodeProcessing += Episode_EpisodeProcessing;
                    episode.Delete();
                }

                OnPodcastSynchronized(podcast.Name);
            }

            var end = DateTime.Now;
            var lapsed = end - start;
            OnSubscriptionSynchronized(Podcasts.Count, lapsed);
        }

        /// <summary>
        /// Copy podcasts from download location to USB key
        /// </summary>
        /// <param name="downloadFolder">folder podcasts are downloaded to</param>
        /// <param name="destinationFolder">folder podcasts are copied to</param>
        /// <param name="group">copy group to use, defaults to null (all)</param>
        public void Copy(string downloadFolder, string destinationFolder, string group = null)
        {
            var start = DateTime.Now;
            //check that the destination folder is (probably) a USB key and has some free space
            var x = new DriveInfo(destinationFolder);
            if (!x.IsReady || x.DriveType != DriveType.Removable || x.AvailableFreeSpace <= 0)
            {
                //drive not ready
                throw new IOException("Destination is not the right type or is not ready");
            }

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

            var folders = Group.GetFolders(group, downloadFolder);
            
            //does group file size exceed max?
            if (Group.ExceedsMaximumSize(group, folders))
            {
                throw new ArgumentException("specified group exceeds maximum size");
            }

            //copy files in each folder to destination
            //if file exists, skip
            var index = 0;
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
                    if (files.Length < 100 && files.Length > 1 && destination.StartsWith("0"))
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
                            OnEpisodeProcessing(EpisodeEventArgs.Action.Copying, file, destination);
                            File.Copy(file, destination, false);
                            OnEpisodeProcessing(EpisodeEventArgs.Action.Copied, file, destination);
                        }
                        else
                        {
                            OnEpisodeProcessing(EpisodeEventArgs.Action.Copied, null, destination);
                        }
                    }
                    catch (Exception)
                    {
                        OnEpisodeProcessing(EpisodeEventArgs.Action.Error, filename, destination);
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
            var end = DateTime.Now;
            var lapsed = end - start;
            OnSubscriptionCopied(index, lapsed);
        }

        #region Event Handlers

        #region Subscription

        /// <summary>
        /// Handler to raise event when a folder is created
        /// </summary>
        /// <param name="path"></param>
        protected virtual void OnFolderCreated(string path)
        {
            FolderCreated?.Invoke(this, new FolderCreatedEventArgs(path));
        }

        /// <summary>
        /// Handler to raise event when opening subscription
        /// </summary>
        /// <param name="count">Number of podcasts in subscription</param>
        protected virtual void OnSubscriptionSynchronizing(int count)
        {
            SubscriptionSynchronizing?.Invoke(this, new SubscriptionEventArgs(count, -1));
        }

        /// <summary>
        /// Handler to raise subscription done with sync event
        /// </summary>
        /// <param name="count">Number of podcasts synchronized</param>
        /// <param name="duration">How long the synchronization took</param>
        protected virtual void OnSubscriptionSynchronized(int count, TimeSpan duration)
        {
            SubscriptionSynchronized?.Invoke(this, new SubscriptionTimedEventArgs(count, duration));
        }

        /// <summary>
        /// Raises subscription copying event
        /// </summary>
        /// <param name="current">The podcast current being copied</param>
        /// <param name="total">Total number of podcasts to copy</param>
        protected virtual void OnSubscriptionCopying(int current, int total)
        {
            SubscriptionCopying?.Invoke(this, new SubscriptionEventArgs(total, current));
        }

        /// <summary>
        /// Raises the subscription copied event
        /// </summary>
        protected virtual void OnSubscriptionCopied(int count, TimeSpan duration)
        {
            SubscriptionCopied?.Invoke(this, new SubscriptionTimedEventArgs(count, duration));
        }

        #endregion

        #region Podcast

        /// <summary>
        ///  Handler to raise event when synchronizing podcast
        /// </summary>
        /// <param name="name">Name of podcast</param>
        /// <param name="url">Podcast address</param>
        /// <param name="episodesToDownload">Number of episodes to download</param>
        /// <param name="episodesToDelete">Number of episodes to delete</param>
        protected virtual void OnPodcastSynchronizing(string name, string url = "?", int episodesToDownload = -1, int episodesToDelete = -1)
        {
            PodcastSynchronizing?.Invoke(this, new PodcastDetailEventArgs(name, url, episodesToDownload, episodesToDelete));
        }

        /// <summary>
        /// Handler to raise event when opening podcast
        /// </summary>
        /// <param name="name">Podcast name</param>
        protected virtual void OnPodcastSynchronized(string name)
        {
            PodcastSynchronized?.Invoke(this, new PodcastDetailEventArgs(name));
        }

        /// <summary>
        /// Raises podcast copying event
        /// </summary>
        /// <param name="name">Name of the podcast being copied</param>
        protected virtual void OnPodcastCopying(string name)
        {
            PodcastCopying?.Invoke(this, new PodcastDetailEventArgs(name));
        }

        /// <summary>
        /// Raises the podcast copied event
        /// </summary>
        /// <param name="name">Name of the podcast being copied</param>
        protected virtual void OnPodcastCopied(string name)
        {
            PodcastCopied?.Invoke(this, new PodcastDetailEventArgs(name));
        }

        #endregion

        #region Episode

        /// <summary>
        /// Catches episode processing event and passes it along to any listeners
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Information about the episode being processed</param>
        protected virtual void Episode_EpisodeProcessing(object sender, EpisodeEventArgs e)
        {
            EpisodeProcessing?.Invoke(sender, new EpisodeEventArgs(e.Activity, e.Name, e.Url, e.Path));
        }

        /// <summary>
        /// Raises episode processing event 
        /// </summary>
        /// <param name="activity">Action taken on this episode</param>
        /// <param name="source">Episode title or name</param>
        /// <param name="destination">Intended path to episode</param>
        protected virtual void OnEpisodeProcessing(EpisodeEventArgs.Action activity, string source, string destination)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(activity, source, "", destination));
        }

        #endregion

        #endregion

    }
}
