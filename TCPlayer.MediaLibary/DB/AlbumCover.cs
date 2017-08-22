using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using TagLib;

namespace TCPlayer.MediaLibary.DB
{
    [Serializable]
    public sealed class AlbumCover
    {
        public byte[] CoverData { get; set; }
        public string ArtitstTitle { get; set; }
        public uint Year { get; set; }

        public void SetFromIPicture(IPicture picture)
        {
            using (var input = new MemoryStream(picture.Data.ToArray()))
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
                    CoverData = output.ToArray();
                }
            }
        }

        public BitmapImage Cover
        {
            get
            {
                if (CoverData == null || CoverData.Length == 0)
                    return null;

                using (var input = new MemoryStream(CoverData))
                {
                    BitmapImage ret = new BitmapImage();
                    ret.BeginInit();
                    ret.StreamSource = input;
                    ret.DecodePixelWidth = 300;
                    ret.EndInit();
                    return ret;
                }
            }
        }
    }
}
