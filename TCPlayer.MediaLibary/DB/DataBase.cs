﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPlayer.MediaLibary.DB
{
    public sealed partial class DataBase : IDisposable
    {
        private LiteDatabase _database;
        private readonly LiteCollection<TrackEntity> _tracks;
        private readonly LiteCollection<AlbumCover> _covers;
        private readonly string _dbpath;


        public DataBase()
        {
            var musicdir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            _dbpath = Path.Combine(musicdir, "TCPlayerDb.db");
            _database = new LiteDatabase(_dbpath);
            _tracks = _database.GetCollection<TrackEntity>("Tracks");
            _tracks.EnsureIndex(x => x.Hash);
            _tracks.EnsureIndex(x => x.Artist);
            _tracks.EnsureIndex(x => x.Title);
            _covers = _database.GetCollection<AlbumCover>("Covers");
            _covers.EnsureIndex(x => x.ArtitstTitle);
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