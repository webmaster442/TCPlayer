using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TCPlayer.Jobs
{
    internal class LoadM3UJob : BasePlaylistLoaderJob
    {
        public override IEnumerable<string> JobFunction(string inputdata, IProgress<float> progress, CancellationToken ct)
        {
            List<string> ret = new List<string>();
            string filedir = Path.GetDirectoryName(inputdata);
            string line;
            int size = 0;
            float i = 0;
            using (var content = LoadFile(inputdata, out size))
            {
                do
                {
                    ct.ThrowIfCancellationRequested();
                    line = content.ReadLine();
                    if (line == null) continue;
                    i += line.Length;
                    if (line.StartsWith("#")) continue;
                    if (line.StartsWith("http://") || line.StartsWith("https://"))
                    {
                        ret.Add(line);
                    }
                    else if (line.Contains(":\\") || line.StartsWith("\\\\"))
                    {
                        if (!File.Exists(line)) continue;
                        ret.Add(line);
                    }
                    else
                    {
                        string f = Path.Combine(filedir, line);
                        if (!File.Exists(f)) continue;
                        ret.Add(f);
                    }
                    progress.Report(i / size);
                }
                while (line != null);
            }
            return ret;
        }
    }
}
