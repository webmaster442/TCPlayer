using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace TCPlayer.MediaLibary.DB
{
    public sealed partial class DataBase : IDisposable
    {
        private static DataBase _Instance;

        private LiteDatabase _database;
        private readonly LiteCollection<TrackEntity> _tracks;
        private readonly LiteCollection<AlbumCover> _covers;
        private readonly LiteCollection<QueryInput> _querys;

        private readonly string _dbpath;

        public Cache DatabaseCache { get; private set; }

        public static DataBase Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new DataBase();
                return _Instance;
            }
        }

        private DataBase()
        {
            var musicdir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            _dbpath = System.IO.Path.Combine(musicdir, "TCPlayerDb.db");
            _database = new LiteDatabase(_dbpath);

            _tracks = _database.GetCollection<TrackEntity>("Tracks");
            _tracks.EnsureIndex(x => x.Hash);
            _tracks.EnsureIndex(x => x.Artist);
            _tracks.EnsureIndex(x => x.Title);

            _covers = _database.GetCollection<AlbumCover>("Covers");
            _covers.EnsureIndex(x => x.ArtitstTitle);

            _querys = _database.GetCollection<QueryInput>("Query");
            _querys.EnsureIndex(x => x.Name);

            DatabaseCache = new Cache(_tracks);
        }

        public void Dispose()
        {
            if (_database != null)
            {
                _database.Dispose();
                _database = null;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Add files to database
        /// </summary>
        /// <param name="filenames">Filenames to add</param>
        public async Task AddFiles(params string[] filenames)
        {
            var errors = new StringBuilder();

            foreach (var file in filenames)
            {
                try
                {
                    using (File f = File.Create(file))
                    {
                        var song = new TrackEntity
                        {
                            AddDate = DateTime.Now,
                            Album = f.Tag.Album,
                            Artist = f.Tag.JoinedPerformers,
                            Comment = f.Tag.Comment,
                            Disc = f.Tag.Disc,
                            Track = f.Tag.Track,
                            FileSize = f.Length,
                            Generire = f.Tag.FirstGenre,
                            Length = f.Properties.Duration.TotalSeconds,
                            LastPlay = DateTime.MinValue,
                            Path = file,
                            PlayCounter = 0,
                            Rating = -1,
                            Year = f.Tag.Year,
                            Title = f.Tag.Title
                        };
                        CalculateHash(ref song);
                        _tracks.Insert(song);
                        await AddCoverIfNotExist(f.Tag);
                    }
                }
                catch (Exception ex)
                {
                    errors.AppendLine(ex.Message);
                }

                if (errors.Length > 0)
                    throw new DBException(errors);
            }

            DatabaseCache.Refresh();
        }

        public IEnumerable<TrackEntity> Execute(QueryInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            IEnumerable<TrackEntity> results = null;

            if (!string.IsNullOrEmpty(input.AlbumName))
            {
               results = _tracks.Find(item => string.Compare(item.Album, input.AlbumName, true) == 0);
            }

            if (!string.IsNullOrEmpty(input.Artist))
            {
                if (results == null)
                    results = _tracks.Find(item => string.Compare(item.Artist, input.Artist, true) == 0);
                else
                    results = results.Where(item => string.Compare(item.Artist, input.Artist, true) == 0);
            }

            if (input.Year != null)
            {
                if (results == null)
                    results = _tracks.Find(item => item.Year == input.Year.Value);
                else
                    results = results.Where(item => item.Year == input.Year.Value);
            }

            return results;
        }


    }
}
