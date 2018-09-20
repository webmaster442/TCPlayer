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
using ManagedBass.Cd;
using System;
using System.Collections.Generic;
using System.Threading;
using TaskRunner;

namespace TCPlayer.Jobs
{
    public class CDGetInfoJob : IJob<string, IEnumerable<string>>
    {
        public IEnumerable<string> JobFunction(string inputdata, IProgress<float> progress, CancellationToken ct)
        {
            var list = new List<string>();

            int drivecount = BassCd.DriveCount;
            int driveindex = 0;
            for (int i = 0; i < drivecount; i++)
            {
                ct.ThrowIfCancellationRequested();
                var info = BassCd.GetInfo(i);
                if (info.DriveLetter == inputdata[0])
                {
                    driveindex = i;
                    break;
                }
            }

            if (BassCd.IsReady(driveindex))
            {
                ct.ThrowIfCancellationRequested();
                var numtracks = BassCd.GetTracks(driveindex);

                if (numtracks < 2) return list;

                var discid = BassCd.GetID(0, CDID.CDDB); //cddb connect
                if (App.DiscID != discid)
                {
                    ct.ThrowIfCancellationRequested();
                    var datas = BassCd.GetIDText(driveindex);
                    App.DiscID = discid;
                    App.CdData.Clear();
                    foreach (var data in datas)
                    {
                        ct.ThrowIfCancellationRequested();
                        var item = data.Split('=');
                        App.CdData.Add(item[0], item[1]);
                    }
                }
                for (int i = 0; i < numtracks; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var entry = string.Format("cd://{0}/{1}", driveindex, i);
                    list.Add(entry);
                    progress.Report((float)i / numtracks);
                }
            }
            BassCd.Release(driveindex);
            return list;
        }
    }
}
