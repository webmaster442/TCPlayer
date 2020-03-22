using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Webmaster442.LibItunesXmlDb.Internals
{
    internal static class Parser
    {
        public static bool ParseBoolean(this XElement track, string keyValue)
        {
            return (from keyNode in track.Descendants("key")
                    where keyNode.Value == keyValue
                    select (keyNode.NextNode as XElement).Name).FirstOrDefault() == "true";
        }

        public static string ParseStringValue(this XElement track, string keyValue)
        {
            return (from key in track.Descendants("key")
                    where key.Value == keyValue
                    select (key.NextNode as XElement).Value).FirstOrDefault();
        }

        public static long ParseLongValue(this XElement track, string keyValue)
        {
            var stringValue = ParseStringValue(track, keyValue);
            if (stringValue == null)
                return -1;
            else
                return long.Parse(stringValue);
        }

        public static int? ParseNullableIntValue(this XElement track, string keyValue)
        {
            var stringValue = ParseStringValue(track, keyValue);
            return String.IsNullOrEmpty(stringValue) ? (int?)null : int.Parse(stringValue);
        }

        public static DateTime? ParseNullableDateValue(this XElement track, string keyValue)
        {
            var stringValue = ParseStringValue(track, keyValue);
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }
            else
            {
                return DateTime.SpecifyKind(DateTime.Parse(stringValue, CultureInfo.InvariantCulture), DateTimeKind.Utc).ToLocalTime();
            }
        }

        public static string MillisecondsToFormattedMinutesAndSeconds(long milliseconds)
        {
            var ts = TimeSpan.FromMilliseconds(milliseconds);
            return ts.ToString("m\\:ss");
        }

        public static string UrlDecode(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            if (url.StartsWith("file://localhost/"))
            {
                url = url.Replace("file://localhost/", "");
                url = url.Replace("/", @"\");
                var replacetable = FillTable();

                url = HttpUtility.UrlDecode(url);

                if (url.Contains("&#"))
                {
                    foreach (var item in replacetable)
                    {
                        if (url.Contains(item.Key))
                        {
                            url = url.Replace(item.Key, item.Value);
                        }
                    }
                }

                return url;
            }
            else
            {
                return url;
            }
        }

        private static Dictionary<string, string> FillTable()
        {
            Dictionary<string, string> decode = new Dictionary<string, string>();
            for (int i=32; i<128; ++i)
            {
                decode.Add($"&#{i};", ((char)i).ToString());
            }
            return decode;
        }

        public static Track CreateTrack(XElement trackElement, bool exludeNotExistingFiles)
        {
            var path = UrlDecode(ParseStringValue(trackElement, "Location"));

            if (exludeNotExistingFiles && !System.IO.File.Exists(path))
            {
                return null;
            }

            return new Track
            {
                TrackId = Int32.Parse(trackElement.ParseStringValue("Track ID")),
                Name = trackElement.ParseStringValue("Name"),
                Artist = trackElement.ParseStringValue("Artist"),
                AlbumArtist = trackElement.ParseStringValue("AlbumArtist"),
                Composer = trackElement.ParseStringValue("Composer"),
                Album = trackElement.ParseStringValue("Album"),
                Genre = trackElement.ParseStringValue("Genre"),
                Kind = trackElement.ParseStringValue("Kind"),
                Size = trackElement.ParseLongValue("Size"),
                PlayingTime = MillisecondsToFormattedMinutesAndSeconds(trackElement.ParseLongValue("Total Time")),
                TrackNumber = trackElement.ParseNullableIntValue("Track Number"),
                Year = trackElement.ParseNullableIntValue("Year"),
                DateModified = trackElement.ParseNullableDateValue("Date Modified"),
                DateAdded = trackElement.ParseNullableDateValue("Date Added"),
                BitRate = trackElement.ParseNullableIntValue("Bit Rate"),
                SampleRate = trackElement.ParseNullableIntValue("Sample Rate"),
                PlayDate = trackElement.ParseNullableDateValue("Play Date UTC"),
                PlayCount = trackElement.ParseNullableIntValue("Play Count"),
                PartOfCompilation = ParseBoolean(trackElement, "Compilation"),
                FilePath = path
            };
        }
    }
}
