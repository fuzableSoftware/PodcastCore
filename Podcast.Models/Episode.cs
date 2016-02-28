﻿using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Fuzable.Podcast.Entities.Episodes;

namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// A podcast episode
    /// </summary>
    public class Episode
    {
        /// <summary>
        /// Title of episode
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Episode URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Path to episode
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Episode processing event
        /// </summary>
        public event EpisodeEventHandler EpisodeProcessing;

        #region Event Handlers

        /// <summary>
        /// Raises EpisodeProcessing event
        /// </summary>
        /// <param name="process">Action on episode</param>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        protected virtual void OnEpisodeProcessing(EpisodeEventArgs.Action process, string name, string url, string path)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(process, name, url, path));
        }

        /// <summary>
        /// Raises EpisodeDownloading event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        protected virtual void OnEpisodeDownloading(string name, string url, string path)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(EpisodeEventArgs.Action.Downloading, name, url, path));
        }

        /// <summary>
        /// Raises EpisodeDownloaded event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Local path to episode</param>
        protected virtual void OnEpisodeDownloaded(string name, string url, string path)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(EpisodeEventArgs.Action.Downloaded, name, url, path));
        }

        /// <summary>
        /// Raises EpisodeDeleted event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="path">Local path to episode</param>
        protected virtual void OnEpisodeDeleted(string name, string path)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(EpisodeEventArgs.Action.Deleted, name, "", path));
        }

        /// <summary>
        /// Raises EpisodeDownloadFailed event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        protected virtual void OnEpisodeDownloadFailed(string name, string url)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(EpisodeEventArgs.Action.Error, name, url));
        }

        /// <summary>
        /// Raises EpisodeDeleteFailed event
        /// </summary>
        /// <param name="name">Name of episode</param>
        protected virtual void OnEpisodeDeleteFailed(string name)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(EpisodeEventArgs.Action.Error, name));
        }

        /// <summary>
        /// Raises EpisodeSynchronized event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        protected virtual void OnEpisodeSynchronized(string name, string url)
        {
            EpisodeProcessing?.Invoke(this, new EpisodeEventArgs(EpisodeEventArgs.Action.Synchronized, name, url));
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Podcast title</param>
        /// <param name="url">Podcast URL</param>
        /// <param name="filepath">Podcast path</param>
        public Episode(string title, string url, string filepath)
        {
            Title = title;
            Url = url;
            FilePath = filepath;
        }

        /// <summary>
        /// Downloads an episode
        /// </summary>
        public void Download()
        {
            if (File.Exists(FilePath))
            {
                OnEpisodeSynchronized(Title, null);
            }
            else
            {
                try
                {
                    if (Url == null) return;
                    OnEpisodeDownloading(Title, Url, FilePath);

                    if (EpisodeIsDownloaded(FilePath))
                    {
                        //we're done
                        OnEpisodeDownloaded(Title, Url, FilePath);
                        return;
                    }

                    //download it 
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(Url, FilePath);
                    }
                    OnEpisodeDownloaded(Title, Url, FilePath);
                }
                catch (WebException)
                {
                    OnEpisodeDownloadFailed(Title, Url);
                    //delete failed download
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                }
                finally
                {
                    OnEpisodeSynchronized(Title, Url);
                }
            }
        }

        /// <summary>
        /// Removes a downloaded episode
        /// </summary>
        public void Delete()
        {
            //if (File.Exists(FilePath))
            //{
            //    OnEpisodeSynchronized(Title, null);
            //}
            //else
            //{
            //    try
            //    {
            //        if (Url == null) return;
            //        OnEpisodeDownloading(Title, Url, FilePath);

            //        if (EpisodeIsDownloaded(FilePath))
            //        {
            //            //we're done
            //            OnEpisodeDownloaded(Title, Url, FilePath);
            //            return;
            //        }

            //        //download it 
            //        using (var client = new WebClient())
            //        {
            //            client.DownloadFile(Url, FilePath);
            //        }
            //        OnEpisodeDownloaded(Title, Url, FilePath);
            //    }
            //    catch (WebException)
            //    {
            //        OnEpisodeDownloadFailed(Title, Url);
            //        //delete failed download
            //        if (File.Exists(FilePath))
            //        {
            //            File.Delete(FilePath);
            //        }
            //    }
            //    finally
            //    {
            //        OnEpisodeSynchronized(Title, Url);
            //    }
            //}
        }

        /// <summary>
        /// generate episode filename from title, intended download folder, download index (order)
        /// </summary>
        /// <param name="title">Episode title</param>
        /// <param name="downloadFolder">Folder to download to</param>
        /// <param name="index">Order episide title</param>
        /// <param name="remove">string to remove from titles, set by podcast</param>
        /// <returns></returns>
        public static string GenerateEpisodeFilename(string title, string downloadFolder, int index, string remove = "")
        {
            //split on colons, remove any spacing
            var parts = title.Split(':').Select(p => p.Trim());
            var filename = string.Join(":", parts.ToArray());
            //replace remove string 
            if (remove != null)
            {
                filename = filename.Replace(remove, "");
            }
            //replace invalid file system chars with dashes
            filename = Path.GetInvalidFileNameChars().Aggregate(filename, (current, c) => current.Replace(c, '-'));
            //remove preceding or trailing dashes
            filename = filename.TrimEnd('-').TrimStart('-');
            //prefix filename with index or order of download
            if (index >= 0)
            {
                filename = index.ToString("000") + "_" + filename;
            }
            //append type
            filename += ".mp3";
            //combine with the path
            filename = Path.Combine(downloadFolder, filename);
            //return
            return filename;
        }

        static string GetFilenameWithoutPrefix(string filename)
        {
            return filename.Substring(4);
        }

        static bool EpisodeIsDownloaded(string fullPath)
        {
            if (fullPath == null)
            {
                return false;
            }

            //split apart path
            var filename = Path.GetFileName(fullPath);
            var path = Path.GetDirectoryName(fullPath);
            if (path == null) return false;
            //does file exist now?
            var partialFilename = GetFilenameWithoutPrefix(filename);
            var folder = new DirectoryInfo(path);
            var files = folder.EnumerateFiles("???_" + partialFilename).ToList();
            var numberOfFiles = files.Count();
            switch (numberOfFiles)
            {
                case 0:
                    //no files found with similar name
                    //just download file or whatever, nothing to do 
                    return false;
                case 1:
                    //1 file found with similar name
                    //rename this file with the new prefix 
                    var file = files[0];
                    var newPath = @Path.Combine(path, filename);
                    file.MoveTo(newPath);
                    //we updated the file, so tell the caller
                    return true;
            }
            //more than 1 file? something's wrong 
            Debug.Assert(numberOfFiles < 2, "too many files returned in similar name search");
            return false;
        }
    }
}
