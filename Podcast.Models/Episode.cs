using System.Diagnostics;
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

        #region Events

        /// <summary>
        /// Episode downloading event
        /// </summary>
        public event EpisodeDownloadingHandler EpisodeDownloading;

        /// <summary>
        /// Episode downloaded event
        /// </summary>
        public event EpisodeDownloadedHandler EpisodeDownloaded;

        /// <summary>
        /// Episode download failed event
        /// </summary>
        public event EpisodeDownloadFailedHandler EpisodeDownloadFailed;

        /// <summary>
        /// Episode download synchronized event
        /// </summary>
        public event EpisodeSynchronizedHandler EpisodeSynchronized;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises EpisodeDownloading event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Episode local path</param>
        protected virtual void OnEpisodeDownloading(string name, string url, string path)
        {
            EpisodeDownloading?.Invoke(this, new EpisodeEventArgs(name, url, path));
        }

        /// <summary>
        /// Raises EpisodeDownloaded event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        /// <param name="path">Local path to episode</param>
        protected virtual void OnEpisodeDownloaded(string name, string url, string path)
        {
            EpisodeDownloaded?.Invoke(this, new EpisodeEventArgs(name, url, path));
        }

        /// <summary>
        /// Raises EpisodeDownloadFailed event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        protected virtual void OnEpisodeDownloadFailed(string name, string url)
        {
            EpisodeDownloadFailed?.Invoke(this, new EpisodeEventArgs(name, url));
        }

        /// <summary>
        /// Raises EpisodeSynchronized event
        /// </summary>
        /// <param name="name">Name of episode</param>
        /// <param name="url">Episode address</param>
        protected virtual void OnEpisodeSynchronized(string name, string url)
        {
            EpisodeSynchronized?.Invoke(this, new EpisodeEventArgs(name, url));
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
