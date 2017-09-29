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
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TagLib;
using AppLib.Common.Extensions;

namespace TCPlayer.MediaLibary.DB
{
    public sealed partial class DataBase
    {
        private void CalculateHash(ref TrackEntity s)
        {
            s.Hash = string.Empty;
            using (var xml = new System.IO.StringWriter())
            {
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(TrackEntity));
                xs.Serialize(xml, s);
                var hash = (new System.Security.Cryptography.SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(xml.ToString()));
                s.Hash = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
            }
        }

        private string AlbumCoverId(Tag t)
        {
            var input = t.FirstAlbumArtist + " - " + t.Album;
            var hash = (new System.Security.Cryptography.SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        private string AlbumCoverId(TrackEntity t)
        {
            var input = t.AlbumArtist + " - " + t.Album;
            var hash = (new System.Security.Cryptography.SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        private Task AddCoverIfNotExist(Tag t)
        {
            return Task.Run(() =>
            {
                if (t.Pictures.Length < 1) return;

                var id = AlbumCoverId(t);

                if (!_database.FileStorage.Exists(id))
                {
                    using (var input = new MemoryStream(t.Pictures[0].Data.ToArray()))
                    {
                        BitmapImage ret = new BitmapImage();
                        ret.BeginInit();
                        ret.StreamSource = input;
                        ret.DecodePixelWidth = 300;
                        ret.EndInit();


                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(ret));

                        using (var output = new MemoryStream())
                        {
                            encoder.Save(output);
                            output.Seek(0, SeekOrigin.Begin);
                            _database.FileStorage.Upload(id, null, output);
                        }
                    }
                }
            });
        }

        private BitmapImage GetCover(TrackEntity t)
        {
            var id = AlbumCoverId(t);

            if (_database.FileStorage.Exists(id))
            {
                using (var stream = new MemoryStream())
                {
                    _database.FileStorage.Download(id, stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    BitmapImage ret = new BitmapImage();
                    ret.BeginInit();
                    ret.StreamSource = stream;
                    ret.DecodePixelWidth = 300;
                    ret.EndInit();

                    return ret;
                }
            }
            else
                return null;

        }

        private void ResoreCacheIfExists()
        {
            var c = _database.GetCollection<Cache>(CollectionCache);
            var ci = c.Find(LiteDB.Query.All(), 0, 1).FirstOrDefault();

            if (ci != null)
                DatabaseCache.RestoreFrom(ci);

        }

        private void WriteCache()
        {
            var c = _database.GetCollection<Cache>(CollectionCache);
            c.Delete(LiteDB.Query.All());
            c.Insert(DatabaseCache);
        }
    }
}
