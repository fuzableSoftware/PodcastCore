using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Fuzable.Podcast.Models
{
    class Podcast
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int EpisodesToKeep { get; set; }
        public List<Episode> EpisodesToDownload { get; set; }
        public List<Episode> EpisodesToDelete { get; set; }
    }
}
