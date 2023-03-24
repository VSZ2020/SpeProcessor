using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeProcessorConsole
{
    public class FileSearcher
    {
        public static List<string> SearchSpectrumFiles(string rootFolder)
        {
            List<string> speFiles = new List<string>(0);
            foreach (string f in Directory.GetFiles(rootFolder))
            {
                if (f.Substring(f.Length - 4, 4) == ".spe")
                {
                    speFiles.Add(Path.Combine(rootFolder, f));
                }
            }
            foreach (string dir in Directory.GetDirectories(rootFolder))
            {
                speFiles.AddRange(SearchSpectrumFiles(dir));
            }
            return speFiles;
        }

        public static List<string> SearchSpectrumFilesAsync(string rootFolder)
        {
            List<string> speFiles = new List<string>(0);
            Parallel.ForEach(Directory.GetFiles(rootFolder), (string f) =>
            {
                if (f.Substring(f.Length - 4, 4) == ".spe")
                {
                    speFiles.Add(Path.Combine(rootFolder, f));
                }
            });

            Parallel.ForEach(Directory.GetDirectories(rootFolder), (string dir) =>
            {
                speFiles.AddRange(SearchSpectrumFiles(dir));
            });

            return speFiles;
        }
    }
}
