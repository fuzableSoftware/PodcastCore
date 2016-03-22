using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Fuzable.Podcast.Entities.Properties;

namespace Fuzable.Podcast.Entities
{
    internal class Group
    {
        public string Name { get; set; }
        public List<Podcast> Podcasts { get; set; }

        public static string[] GetFolders(string group, string downloadFolder)
        {
            if (group == null)
            {
                //get folders in download folder
                return Directory.GetDirectories(downloadFolder);
            }
            //get for group
            //read podcasts.xml file and extract groups podcasts from
            var folders = new List<Podcast>();

            try
            {
                var root = XElement.Load($@"{Environment.CurrentDirectory}\{"Podcasts.xml"}");
                var items = from item in root.Descendants("Group")
                    where (string) item.Element("Name") == @group
                    select item.Element("Podcasts");

                folders.AddRange(items.Descendants("Name").Select(podcast => new Podcast(podcast.Value)));
            }
            catch (Exception ex)
            {
                var error = new ApplicationException("Error retrieving groups", ex);
                throw error;
            }
            //filter
            var includedFolders = folders.Select(s => Path.Combine(downloadFolder, s.Name)).ToArray();
            return includedFolders;
        }

        public static bool ExceedsMaximumSize(string group, string[] downloadFolders)
        {
            var max = Settings.Default.MaximumGroupSize;
            if (group == null)
            {
                //all files and folders in download folder
                var info = new DirectoryInfo(downloadFolders[0]);
                var size = info.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
                return size > max;
            }
            var total = downloadFolders.Select(downloadFolder => new DirectoryInfo(downloadFolder)).Select(info => info.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length)).Sum();
            return total > max;
        }
    }
}
