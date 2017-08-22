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

        /// <summary>
        /// Add files to database
        /// </summary>
        /// <param name="filenames">Filenames to add</param>
        public async Task AddFiles(params string[] filenames)
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

                if (errors.Length > 0)
                    throw new DBException(errors);
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
