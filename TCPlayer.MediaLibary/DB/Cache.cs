using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPlayer.MediaLibary.DB
{
    public sealed class Cache
    {
        public List<string> Artists { get; private set; }
        public List<string> Albums { get; private set; }
        public List<string> Geneires { get; private set; }
        public List<uint> Years { get; private set; }

        private LiteCollection<TrackEntity> _dbref;

        public Cache(LiteCollection<TrackEntity> database)
        {
            Artists = new List<string>();
            Albums = new List<string>();
            Geneires = new List<string>();
            Years = new List<uint>();
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
    }
}
