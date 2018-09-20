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
