using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Webmaster442.LibItunesXmlDb.Internals;

namespace Webmaster442.LibItunesXmlDb
{
    /// <summary>
    /// A class for Interacting iTunes xml database 
    /// </summary>
    public class ITunesXmlDb: IITunesXmlDb
    {
        private XDocument _xml;
        private List<Track> _tracks;
        private ITunesXmlDbOptions _options;

        #region ctor
        /// <summary>
        /// Load an iTunes XML File database
        /// </summary>
        /// <param name="fileLocation">full path of iTunes Music Library.xml</param>
        /// <param name="options">Parser options. If not specified default options will be used.</param>
        /// <seealso cref="ITunesXmlDbOptions"/>
        public ITunesXmlDb(string fileLocation, ITunesXmlDbOptions options)
        {
            _xml = XDocument.Load(fileLocation);
            _options = options;
        }
        #endregion

        private IEnumerable<XElement> LoadTrackElements()
        {
            return from x in _xml.Descendants("dict")
                   .Descendants("dict")
                   .Descendants("dict")
                   where x.Descendants("key").Count() > 1
                   select x;
        }

        private IEnumerable<XElement> LoadPlaylists()
        {
            return from x in _xml.Descendants("dict")
                   .Descendants("array")
                   .Descendants("dict")
                   where x.Descendants("key").Count() > 1
                   select x;
        }

        /// <inheritdoc/>
        public IEnumerable<Track> Tracks
        {
            get
            {
                if (_tracks == null)
                {
                    var trackElements = LoadTrackElements();

                    var query = from trackElement in LoadTrackElements()
                                select Parser.CreateTrack(trackElement, _options.ExcludeNonExistingFiles);

                    if (_options.ParalelParsingEnabled)
                    {
                        _tracks = query.Where(x => x != null).AsParallel().ToList();
                    }
                    else
                    {
                        _tracks = query.Where(x => x != null).ToList();
                    }
                }
                return _tracks;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Albums
        {
            get { return Tracks.Select(t => t.Album).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Artists
        {
            get { return Tracks.Select(t => t.Artist).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Genres
        {
            get { return Tracks.Select(t => t.Genre).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Years
        {
            get { return Tracks.Select(t => t.Year.ToString()).OrderBy(t => t).Distinct(); }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Playlists
        {
            get
            {
                var playlistNodes = LoadPlaylists();
                foreach (var item in playlistNodes)
                {
                    yield return item.ParseStringValue("Name");
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Track> Filter(FilterKind kind, string param)
        {
            switch (kind)
            {
                case FilterKind.Album:
                    return Tracks.Where(t => t.Album == param);
                case FilterKind.Artist:
                    return Tracks.Where(t => t.Artist == param);
                case FilterKind.Genre:
                    return Tracks.Where(t => t.Genre == param);
                case FilterKind.Year:
                    return Tracks.Where(t => t.Year == int.Parse(param));
                case FilterKind.None:
                default:
                    return Tracks;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Track> ReadPlaylist(string id)
        {
            var playlistNodes = LoadPlaylists();

            var query = from node in playlistNodes
                        where node.ParseStringValue("Name") == id
                        select node.Descendants("array").Descendants("dict");

            foreach (var item in query)
            {
                foreach (var subitem in item)
                {
                    var trackid = Int32.Parse(subitem.ParseStringValue("Track ID"));
                    var track = Tracks.Where(t => t.TrackId == trackid).FirstOrDefault();
                    yield return track;
                }
            }
        }

        #region static Helpers
        /// <summary>
        /// Return the default user specific path for iTunes Music Library.xml
        /// </summary>
        public static string UserItunesDbPath
        {
            get
            {
                var musicfolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                return System.IO.Path.Combine(musicfolder, @"iTunes\iTunes Music Library.xml");
            }
        }

        /// <summary>
        /// Returns true, if the user has a iTunes Music Library.xml at the default location
        /// </summary>
        public static bool UserHasItunesDb
        {
            get { return System.IO.File.Exists(UserItunesDbPath); }
        }
        #endregion
    }
}
