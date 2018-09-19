using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace TCPlayer.Jobs
{
    internal class LoadPLSJob : BasePlaylistLoaderJob
    {
        public override IEnumerable<string> JobFunction(string inputdata, IProgress<float> progress, CancellationToken ct)
        {
            string filedir = Path.GetDirectoryName(inputdata);
            List<string> ret = new List<string>();
            string line;
            string pattern = @"^(File)([0-9])+(=)";
            int size;
            float i = 0;
            using (var content = LoadFile(inputdata, out size))
            {
                do
                {
                    ct.ThrowIfCancellationRequested();
                    line = content.ReadLine();
                    if (line == null) continue;
                    i += line.Length;
                    if (Regex.IsMatch(line, pattern)) line = Regex.Replace(line, pattern, "");
                    else continue;
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
                return ret;
            }
        }
    }
}
