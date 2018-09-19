using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace TCPlayer.Jobs
{
    internal class LoadWPLJob : BasePlaylistLoaderJob
    {
        public override IEnumerable<string> JobFunction(string inputdata, IProgress<float> progress, CancellationToken ct)
        {
            int size;
            var content = LoadFile(inputdata, out size);
            var doc = XDocument.Load(content).Descendants("body").Elements("seq").Elements("media").ToList();
            List<string> ret = new List<string>(doc.Count);
            float i = 0;
            foreach (var media in doc)
            {
                ct.ThrowIfCancellationRequested();
                var src = media.Attribute("src").Value;
                ret.Add(src);
                i += 1;
                progress.Report(i / ret.Count);
            }
            return ret;
        }
    }
}
