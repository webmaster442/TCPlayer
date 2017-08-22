using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPlayer.MediaLibary.DB
{
    [Serializable]
    public sealed class TrackEntity
    {
        public uint Year { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Album { get; set; }

        public uint Track { get; set; }

        public uint Disc { get; set; }

        public string Generire { get; set; }

        public string Comment { get; set; }

        public DateTime AddDate { get; set; }

        public string Path { get; set; }

        public double Length { get; set; }

        public long FileSize { get; set; }

        public string Hash { get; set; }

        public uint PlayCounter { get; set; }

        public short Rating { get; set; }

        public DateTime LastPlay { get; set; }

        public TrackEntity()
        {
            AddDate = DateTime.Now;
        }
    }
}
