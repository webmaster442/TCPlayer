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
using System.Linq;
using System.Threading;

namespace TCPlayer.Jobs
{
    internal class ImportFromMassStorageJob : BasePlaylistLoaderJob
    {
        private static List<string> Traverse(string directory, CancellationToken ct)
        {
            var stack = new Stack<string>();
            List<string> result = new List<string>();
            stack.Push(directory);
            while (stack.Any())
            {
                ct.ThrowIfCancellationRequested();
                var next = stack.Pop();
                result.Add(next);
                foreach (var child in Directory.GetDirectories(next))
                {
                    stack.Push(child);
                    ct.ThrowIfCancellationRequested();
                }
            }
            return result;
        }


        public override IEnumerable<string> JobFunction(string inputdata, IProgress<float> progress, CancellationToken ct)
        {
            string[] filters = App.Formats.Split(';');
            List<string> result = new List<string>();
            var directories = Traverse(inputdata, ct);
            float i = 0;
            foreach (var directory in directories)
            {
                ct.ThrowIfCancellationRequested();
                foreach (var filter in filters)
                {
                    ct.ThrowIfCancellationRequested();
                    result.AddRange(Directory.GetFiles(directory, filter));
                }
                i += 1;
                progress.Report(i / directories.Count);
            }

            return result;
        }
    }
}
