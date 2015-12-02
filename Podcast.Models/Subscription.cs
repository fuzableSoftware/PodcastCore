using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// Manages subscription file (podcasts.xml)
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Returns podcasts in subscription file
        /// </summary>
        /// <returns></returns>
        public static List<Podcast> GetPodcasts()
        {
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
                Console.WriteLine("Error retrieving subscriptions");
                Console.WriteLine(ex);
                Console.ReadLine();
            }

            return podcasts;
        }
    }
}
