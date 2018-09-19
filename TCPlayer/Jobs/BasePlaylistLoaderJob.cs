using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using TaskRunner;

namespace TCPlayer.Jobs
{
    internal abstract class BasePlaylistLoaderJob : IJob<string, IEnumerable<string>>
    {
        protected static TextReader LoadFile(string file, out int size)
        {
            if (file.StartsWith("http://") || file.StartsWith("https://"))
            {
                try
                {
                    using (var client = new System.Net.WebClient())
                    {
                        var response = client.DownloadString(new Uri(file));
                        size = response.Length;
                        return new StringReader(response);
                    }
                }
                catch (WebException)
                {
                    size = 0;
                    return null;
                }
            }
            else
            {
                size = (int)new FileInfo(file).Length;
                return File.OpenText(file);
            }
        }

        public abstract IEnumerable<string> JobFunction(string inputdata, IProgress<float> progress, CancellationToken ct);
    }
}
