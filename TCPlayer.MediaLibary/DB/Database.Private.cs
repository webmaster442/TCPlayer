using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

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

        private Task AddCoverIfNotExist(Tag t)
        {
            return Task.Run(() =>
            {
                if (t.Pictures.Length < 1) return;

                var searchkey = string.Format("{0} - {1}", t.JoinedPerformers, t.Album);
                var exists = _covers.Find(x => x.ArtitstTitle == searchkey).FirstOrDefault();
                if (exists != null) return;

                var cover = new AlbumCover
                {
                    ArtitstTitle = searchkey,
                    Year = t.Year
                };
                cover.SetFromIPicture(t.Pictures[0]);

                _covers.Insert(cover);
            });
        }
    }
}
