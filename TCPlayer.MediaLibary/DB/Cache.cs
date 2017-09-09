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
using AppLib.Common.Extensions;
using LiteDB;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TCPlayer.MediaLibary.DB
{
    [Serializable]
    public sealed class Cache
    {
        public ObservableCollection<string> Artists { get; private set; }
        public ObservableCollection<string> Albums { get; private set; }
        public ObservableCollection<string> Geneires { get; private set; }
        public ObservableCollection<uint> Years { get; private set; }

        private LiteCollection<TrackEntity> _dbref;

        public Cache()
        {
            Artists = new ObservableCollection<string>();
            Albums = new ObservableCollection<string>();
            Geneires = new ObservableCollection<string>();
            Years = new ObservableCollection<uint>();
        }

        public Cache(LiteCollection<TrackEntity> database) : this()
        {
            _dbref = database;
        }

        public void Clear()
        {
            Artists.Clear();
            Albums.Clear();
            Geneires.Clear();
            Years.Clear();
        }

        public void Refresh()
        {
            Clear();

            var artists = (from i in _dbref.FindAll()
                           orderby i.Artist ascending
                           select i.Artist).Distinct();
            Artists.AddRange(artists);

            var albums = (from i in _dbref.FindAll()
                          orderby i.Album ascending
                          select i.Album).Distinct();
            Albums.AddRange(albums);

            var geneires = (from i in _dbref.FindAll()
                            orderby i.Generire ascending
                            select i.Generire).Distinct();
            Geneires.AddRange(geneires);

            var years = (from i in _dbref.FindAll()
                         orderby i.Year ascending
                         select i.Year).Distinct();
            Years.AddRange(years);
        }

        public void RestoreFrom(Cache ci)
        {
            Clear();
            Albums.AddRange(ci.Albums);
            Artists.AddRange(ci.Artists);
            Years.AddRange(ci.Years);
            Geneires.AddRange(ci.Geneires);
        }
    }
}
