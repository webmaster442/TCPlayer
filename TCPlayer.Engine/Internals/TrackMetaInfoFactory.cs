using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TCPlayer.Engine.Domain;
using System.Linq;

namespace TCPlayer.Engine.Internals
{
    internal static class TrackMetaInfoFactory
    {
        public static TrackMetaInfo CreateTrackerInfo(string filePath, int handle)
        {
            try
            {
                return new TrackMetaInfo
                {
                    FileName = filePath,
                    Artist = Marshal.PtrToStringAuto(Bass.ChannelGetTags(handle, TagType.MusicAuth)),
                    Title = Marshal.PtrToStringAuto(Bass.ChannelGetTags(handle, TagType.MusicName)),
                    FileSize = new FileInfo(filePath).Length
                };
            }
            catch (Exception)
            {
                return new TrackMetaInfo();
            }
        }

        public static TrackMetaInfo CreateFileInfo(string filePath)
        {
            try
            {
                TagLib.File tags = TagLib.File.Create(filePath);

                var ret = new TrackMetaInfo
                {
                    FileName = filePath,
                    FileSize = new FileInfo(filePath).Length,
                    Title = tags.Tag.Title,
                    Album = tags.Tag.Album,
                };

                if (tags.Tag.Performers != null &&
                    tags.Tag.Performers.Length != 0)
                {
                    ret.Artist = tags.Tag.Performers[0];
                }

                if (tags.Tag.Pictures.Length > 0)
                {
                    ret.CoverData = tags.Tag.Pictures[0].Data.ToArray();
                }

                return ret;
            }
            catch (Exception)
            {
                return new TrackMetaInfo();
            }
        }


    }
}
