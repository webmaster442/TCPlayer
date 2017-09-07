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
