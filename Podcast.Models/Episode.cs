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

        /// <summary>
        /// Episode processing event
        /// </summary>
        public event EpisodeEventHandler EpisodeProcessing;

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
                OnEpisodeProcessing(EpisodeEventArgs.Action.Synchronized, Title, null, FilePath);
            }
            else
            {
                try
                {
                    if (Url == null) return;
                    OnEpisodeProcessing(EpisodeEventArgs.Action.Downloading, Title, Url, FilePath);

                    if (EpisodeIsDownloaded(FilePath))
                    {
                        //we're done
                        OnEpisodeProcessing(EpisodeEventArgs.Action.Downloaded, Title, Url, FilePath);
                        return;
                    }

                    //download it 
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(Url, FilePath);
                    }
                    OnEpisodeProcessing(EpisodeEventArgs.Action.Downloaded, Title, Url, FilePath);
                }
                catch (WebException)
                {
                    OnEpisodeProcessing(EpisodeEventArgs.Action.Error, Title, Url, "");
                    //delete failed download
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                }
                finally
                {
                    OnEpisodeProcessing(EpisodeEventArgs.Action.Synchronized, Title, FilePath, Url);
                }
            }
        }

        /// <summary>
        /// Removes a downloaded episode
        /// </summary>
        public void Delete()
        {
            if (EpisodeWasDeleted(FilePath))
            {
                OnEpisodeProcessing(EpisodeEventArgs.Action.Deleted, Title, Url, FilePath);
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
            if (filename == null )
            {
                return null;
            }
            int i;
            return int.TryParse(filename.Substring(1, 1), out i) ? filename.Substring(4) : filename;
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

        static bool EpisodeWasDeleted(string fullPath)
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
            var files = folder.EnumerateFiles("*" + partialFilename).ToList();
            var numberOfFiles = files.Count();
            switch (numberOfFiles)
            {
                case 0:
                    //no files found with similar name
                    //nothing to do 
                    return false;
                case 1:
                    //1 file found with similar name
                    //delete this file 
                    var file = files[0];
                    file.Delete();
                    //we deleted the file, so tell the caller
                    return true;
            }
            //more than 1 file? something's wrong 
            Debug.Assert(numberOfFiles < 2, "too many files returned in similar name search");
            return false;
        }
    }
}
