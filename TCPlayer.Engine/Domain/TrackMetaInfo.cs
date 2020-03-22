using System;
using System.Collections.Generic;

namespace TCPlayer.Engine.Domain
{
    public sealed class TrackMetaInfo : IEquatable<TrackMetaInfo>
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string FileName { get; set; }
        public byte[] CoverData { get; set; }
        public long FileSize { get; set; }

        public TrackMetaInfo()
        {
            Artist = string.Empty;
            Title = string.Empty;
            Album = string.Empty;
            FileName = string.Empty;
            CoverData = new byte[0];
            FileSize = 0L;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TrackMetaInfo);
        }

        public bool Equals(TrackMetaInfo other)
        {
            return other != null &&
                   Artist == other.Artist &&
                   Title == other.Title &&
                   Album == other.Album &&
                   FileName == other.FileName &&
                   EqualityComparer<byte[]>.Default.Equals(CoverData, other.CoverData) &&
                   FileSize == other.FileSize;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Artist, Title, Album, FileName, CoverData, FileSize);
        }

        public static bool operator ==(TrackMetaInfo left, TrackMetaInfo right)
        {
            return EqualityComparer<TrackMetaInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(TrackMetaInfo left, TrackMetaInfo right)
        {
            return !(left == right);
        }
    }
}
