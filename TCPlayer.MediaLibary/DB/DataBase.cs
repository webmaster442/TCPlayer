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
        private readonly LiteCollection<QueryInput> _querys;

        private readonly string _dbpath;

        private const string CollectionTracks = "Tracks";
        private const string CollectionQuery = "Query";
        private const string CollectionCache = "Cache";

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

            _tracks = _database.GetCollection<TrackEntity>(CollectionTracks);
            _tracks.EnsureIndex(x => x.Hash);
            _tracks.EnsureIndex(x => x.Artist);
            _tracks.EnsureIndex(x => x.Title);

            _querys = _database.GetCollection<QueryInput>(CollectionQuery);
            _querys.EnsureIndex(x => x.Name);

            DatabaseCache = new Cache(_tracks);
            ResoreCacheIfExists();
        }

        public void UpdateTrackPlayInfo(TrackEntity selectedTrack)
        { 
            selectedTrack.LastPlay = DateTime.Now;
            selectedTrack.PlayCounter += 1;
           // _tracks.Update(selectedTrack);
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

        public Task AddFiles(params string[] filenames)
        {
            return AddFiles(filenames.AsEnumerable());
        }

        /// <summary>
        /// Add files to database
        /// </summary>
        /// <param name="filenames">Filenames to add</param>
        public async Task AddFiles(IEnumerable<string> filenames)
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
                            AlbumArtist = f.Tag.FirstAlbumArtist,
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
            }

            DatabaseCache.Refresh();
            WriteCache();

            if (errors.Length > 0)
                throw new DBException(errors);
        }

        /// <summary>
        /// Execute a Query
        /// </summary>
        /// <param name="input">Query Input to execute</param>
        /// <returns>Tracks that match the input</returns>
        public IEnumerable<TrackEntity> Execute(QueryInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            IEnumerable<TrackEntity> results = null;

            if (!string.IsNullOrEmpty(input.AlbumName))
            {
                results = _tracks.Find(item => item.Album == input.AlbumName);
            }

            if (!string.IsNullOrEmpty(input.Artist))
            {
                if (results == null)
                    results = _tracks.Find(item => item.Artist == input.Artist);
                else
                    results = results.Where(item => item.Artist == input.Artist);
            }

            if (input.Year != null)
            {
                if (results == null)
                    results = _tracks.Find(item => item.Year == input.Year.Value);
                else
                    results = results.Where(item => item.Year == input.Year.Value);
            }

            if (!string.IsNullOrEmpty(input.Geneire))
            {
                if (results == null)
                    results = _tracks.Find(item => item.Generire == input.Geneire);
                else
                    results = results.Where(item => item.Generire == input.Geneire);
            }

            if (input.Rating != null)
            {
                if (results == null)
                {
                    switch (input.RatingOperator)
                    {
                        case QueryOperator.Equals:
                            results = _tracks.Find(item => item.Rating == input.Rating.Value);
                            break;
                        case QueryOperator.Greater:
                            results = _tracks.Find(item => item.Rating > input.Rating.Value);
                            break;
                        case QueryOperator.GreaterOrEqual:
                            results = _tracks.Find(item => item.Rating >= input.Rating.Value);
                            break;
                        case QueryOperator.Less:
                            results = _tracks.Find(item => item.Rating < input.Rating.Value);
                            break;
                        case QueryOperator.LessOrEqual:
                            results = _tracks.Find(item => item.Rating <= input.Rating.Value);
                            break;
                    }
                }
                else
                {
                    switch (input.RatingOperator)
                    {
                        case QueryOperator.Equals:
                            results = results.Where(item => item.Rating == input.Rating.Value);
                            break;
                        case QueryOperator.Greater:
                            results = results.Where(item => item.Rating > input.Rating.Value);
                            break;
                        case QueryOperator.GreaterOrEqual:
                            results = results.Where(item => item.Rating >= input.Rating.Value);
                            break;
                        case QueryOperator.Less:
                            results = results.Where(item => item.Rating < input.Rating.Value);
                            break;
                        case QueryOperator.LessOrEqual:
                            results = results.Where(item => item.Rating <= input.Rating.Value);
                            break;
                    }
                }
            }

            if (input.PlayCount != null)
            {
                if (results == null)
                {
                    switch (input.PlayCounterOperator)
                    {
                        case QueryOperator.Equals:
                            results = _tracks.Find(item => item.PlayCounter == input.PlayCount.Value);
                            break;
                        case QueryOperator.Greater:
                            results = _tracks.Find(item => item.PlayCounter > input.PlayCount.Value);
                            break;
                        case QueryOperator.GreaterOrEqual:
                            results = _tracks.Find(item => item.PlayCounter >= input.PlayCount.Value);
                            break;
                        case QueryOperator.Less:
                            results = _tracks.Find(item => item.PlayCounter < input.PlayCount.Value);
                            break;
                        case QueryOperator.LessOrEqual:
                            results = _tracks.Find(item => item.PlayCounter <= input.PlayCount.Value);
                            break;
                    }
                }
                else
                {
                    switch (input.PlayCounterOperator)
                    {
                        case QueryOperator.Equals:
                            results = results.Where(item => item.PlayCounter == input.PlayCount.Value);
                            break;
                        case QueryOperator.Greater:
                            results = results.Where(item => item.PlayCounter > input.PlayCount.Value);
                            break;
                        case QueryOperator.GreaterOrEqual:
                            results = results.Where(item => item.PlayCounter >= input.PlayCount.Value);
                            break;
                        case QueryOperator.Less:
                            results = results.Where(item => item.PlayCounter < input.PlayCount.Value);
                            break;
                        case QueryOperator.LessOrEqual:
                            results = results.Where(item => item.PlayCounter <= input.PlayCount.Value);
                            break;
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// List saved queries
        /// </summary>
        /// <returns>List of saved queries</returns>
        public IEnumerable<string> ListSavedQueries()
        {
            return _querys.FindAll().Select(q => q.Name);
        }

        /// <summary>
        /// Save or update a query
        /// </summary>
        /// <param name="query"></param>
        public void SaveOrUpdateQuery(QueryInput query)
        {
            var exists = _querys.Find(stored => stored.Name == query.Name).FirstOrDefault();
            if (exists != null)
                _querys.Delete(d => d.Name == query.Name);

            _querys.Insert(query);
        }
    }
}
