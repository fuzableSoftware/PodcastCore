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
        public string  FilePath { get; set; }

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
    }

}
