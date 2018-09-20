/*
    TC Plyer
    Total Commander Audio Player plugin & standalone player written in C#, based on bass.dll components
    Copyright (C) 2016 Webmaster442 aka. Ruzsinszki Gábor

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
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
