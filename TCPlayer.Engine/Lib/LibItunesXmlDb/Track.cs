using System;
using System.Collections.Generic;

namespace Webmaster442.LibItunesXmlDb
{
    /// <summary>
    /// A class representing tracks in the iTunes Database
    /// </summary>
    public class Track : IEquatable<Track>
    {
        /// <summary>
        /// Track Id
        /// </summary>
        public int TrackId { get; set; }
        /// <summary>
        /// Track Title
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Track Artist
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// Track Album Artist
        /// </summary>
        public string AlbumArtist { get; set; }
        /// <summary>
        /// Track Composer
        /// </summary>
        public string Composer { get; set; }
        /// <summary>
        /// Track Album
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// Track Genre
        /// </summary>
        public string Genre { get; set; }
        /// <summary>
        /// Track Kind
        /// </summary>
        public string Kind { get; set; }
        /// <summary>
        /// Track size in bytes
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// Track length
        /// </summary>
        public string PlayingTime { get; set; }
        /// <summary>
        /// Track number
        /// </summary>
        public int? TrackNumber { get; set; }
        /// <summary>
        /// Track year
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime? DateModified { get; set; }
        /// <summary>
        /// Date added
        /// </summary>
        public DateTime? DateAdded { get; set; }
        /// <summary>
        /// Track bitrate
        /// </summary>
        public int? BitRate { get; set; }
        /// <summary>
        /// Track sample rate
        /// </summary>
        public int? SampleRate { get; set; }
        /// <summary>
        /// Play count
        /// </summary>
        public int? PlayCount { get; set; }
        /// <summary>
        /// Last play date
        /// </summary>
        public DateTime? PlayDate { get; set; }
        /// <summary>
        /// Part of compilation flag
        /// </summary>
        public bool PartOfCompilation { get; set; }
        /// <summary>
        /// File Path
        /// </summary>
        public string FilePath { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as Track);
        }

        /// <inheritdoc/>
        public bool Equals(Track other)
        {
            return other != null &&
                   TrackId == other.TrackId &&
                   Name == other.Name &&
                   Artist == other.Artist &&
                   AlbumArtist == other.AlbumArtist &&
                   Composer == other.Composer &&
                   Album == other.Album &&
                   Genre == other.Genre &&
                   Kind == other.Kind &&
                   Size == other.Size &&
                   PlayingTime == other.PlayingTime &&
                   EqualityComparer<int?>.Default.Equals(TrackNumber, other.TrackNumber) &&
                   EqualityComparer<int?>.Default.Equals(Year, other.Year) &&
                   EqualityComparer<DateTime?>.Default.Equals(DateModified, other.DateModified) &&
                   EqualityComparer<DateTime?>.Default.Equals(DateAdded, other.DateAdded) &&
                   EqualityComparer<int?>.Default.Equals(BitRate, other.BitRate) &&
                   EqualityComparer<int?>.Default.Equals(SampleRate, other.SampleRate) &&
                   EqualityComparer<int?>.Default.Equals(PlayCount, other.PlayCount) &&
                   EqualityComparer<DateTime?>.Default.Equals(PlayDate, other.PlayDate) &&
                   PartOfCompilation == other.PartOfCompilation &&
                   FilePath == other.FilePath;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 404681566;
            hashCode = hashCode * -1521134295 + TrackId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Artist);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AlbumArtist);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Composer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Album);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Genre);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Kind);
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PlayingTime);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(TrackNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(Year);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(DateModified);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(DateAdded);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(BitRate);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(SampleRate);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(PlayCount);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(PlayDate);
            hashCode = hashCode * -1521134295 + PartOfCompilation.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FilePath);
            return hashCode;
        }

        /// <summary>
        /// Compares two instances of Track for equality
        /// </summary>
        /// <param name="track1">instance to compare</param>
        /// <param name="track2">other instance to compare</param>
        /// <returns>true, if the two instances are equal, false if not</returns>
        public static bool operator ==(Track track1, Track track2)
        {
            return EqualityComparer<Track>.Default.Equals(track1, track2);
        }

        /// <summary>
        /// Compares two instances of Track for inequality
        /// </summary>
        /// <param name="track1">instance to compare</param>
        /// <param name="track2">other instance to compare</param>
        /// <returns>false, if the two instances are not equal, false if they are</returns>
        public static bool operator !=(Track track1, Track track2)
        {
            return !(track1 == track2);
        }
    }
}
