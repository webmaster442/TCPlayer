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
