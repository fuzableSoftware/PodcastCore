using System.Collections.Generic;

namespace Fuzable.Podcast.Entities
{
    /// <summary>
    /// A group of podcasts to copy
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Name of podcast copy group
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of podcasts to copy in this group
        /// </summary>
        public List<Podcast> Podcasts { get; set; }
    }
}
