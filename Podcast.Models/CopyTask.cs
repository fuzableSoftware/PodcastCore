using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fuzable.Podcast.Entities
{
    internal class CopyTask
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string FileName { get; set; }
        public bool ExistsAtDestination { get; private set; }
        public string FilenameAtDestination { get; private set; }

        public CopyTask(string source, string destination)
        {
            Source = source;
            Destination = destination;
            FileName = Path.GetFileName(destination);
            ExistsAtDestination = CanShortCircuit();
        }

        bool CanShortCircuit()
        {
            var path = Path.GetDirectoryName(Destination);
            if (path == null) return false;
          
            //does file exist now?
            var partialFilename = Episode.GetFilenameWithoutPrefix(FileName);
            var folder = new DirectoryInfo(path);
            var files = folder.EnumerateFiles("*" + partialFilename).ToList();
            var numberOfFiles = files.Count();
            switch (numberOfFiles)
            {
                case 0:
                    //no files found with similar name
                    return false;
                case 1:
                    //file found with similar name
                    FilenameAtDestination = files[0].ToString();
                    return true;
            }
            //more than 1 file? something's wrong 
            Debug.Assert(numberOfFiles < 2, "too many files returned in similar name search");
            return false;
        }

        //copy or rename
        //return true if copy, false if rename
        internal bool Copy()
        {
            if (ExistsAtDestination)
            {
                //exists, so rename it 
                var path = Path.GetDirectoryName(Destination) ?? "";
                var currentPath = Path.Combine(path, FilenameAtDestination);
                var newPath = Path.Combine(path, FileName);
                File.Move(currentPath, newPath);
                return false;
            }
            //doesn't exist, so copy it 
            File.Copy(Source, Destination);
            return true;
        }
    }
}
