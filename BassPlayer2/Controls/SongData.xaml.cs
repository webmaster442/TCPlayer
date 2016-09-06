using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for SongData.xaml
    /// </summary>
    public partial class SongData : UserControl
    {
        public SongData()
        {
            InitializeComponent();
        }

        public static DependencyProperty CoverProperty =
            DependencyProperty.Register("Cover", typeof(ImageSource), typeof(SongData), new PropertyMetadata(new BitmapImage(new Uri("pack://application:,,,/BassPlayer2;component/Style/unknown.png"))));

        public static DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(SongData));

        public static DependencyProperty ArtistProperty =
            DependencyProperty.Register("Artist", typeof(string), typeof(SongData));

        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SongData));

        public static DependencyProperty AlbumProperty =
            DependencyProperty.Register("Album", typeof(string), typeof(SongData));

        public static DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(string), typeof(SongData));

        public static DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(string), typeof(SongData));

        public ImageSource Cover
        {
            get { return (ImageSource)GetValue(CoverProperty); }
            set { SetValue(CoverProperty, value); }
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public string Artist
        {
            get { return (string)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Album
        {
            get { return (string)GetValue(AlbumProperty); }
            set { SetValue(AlbumProperty, value); }
        }

        public string Year
        {
            get { return (string)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }

        public string Size
        {
            get { return (string)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private string GetFileSize(long value)
        {
            double val = System.Convert.ToDouble(value);
            string unit = "Byte";
            if (val > 1099511627776D)
            {
                val /= 1099511627776D;
                unit = "TiB";
            }
            else if (val > 1073741824D)
            {
                val /= 1073741824D;
                unit = "GiB";
            }
            else if (val > 1048576D)
            {
                val /= 1048576D;
                unit = "MiB";
            }
            else if (val > 1024D)
            {
                val /= 1024D;
                unit = "kiB";
            }
            return string.Format("{0:0.000} {1}", val, unit);
        }

        public void UpdateMediaInfo(string file)
        {
            if (file.StartsWith("http://") || file.StartsWith("https://"))
            {
                FileName = file;
                Cover = new BitmapImage(new Uri("/BassPlayer2;component/Style/network.png", UriKind.Relative));
                Year = DateTime.Now.Year.ToString();
                Album = "";
                Title = "Stream";
                Artist = Path.GetFileName(file);
                Size = "Infinite";
                return;
            }
            FileName = Path.GetFileName(file);
            try
            {
                var fi = new FileInfo(file);
                Size = GetFileSize(fi.Length);

                TagLib.File tags = TagLib.File.Create(file);
                if (tags.Tag.Pictures.Length > 0)
                {
                    var picture = tags.Tag.Pictures[0].Data;
                    MemoryStream ms = new MemoryStream(picture.Data);
                    BitmapImage ret = new BitmapImage();
                    ret.BeginInit();
                    ret.StreamSource = ms;
                    ret.DecodePixelWidth = 200;
                    ret.CacheOption = BitmapCacheOption.OnLoad;
                    ret.EndInit();
                    ms.Close();
                    Cover = ret;
                    Year = tags.Tag.Year.ToString();
                    Artist = tags.Tag.Performers[0];
                    Album = tags.Tag.Album;
                    Title = tags.Tag.Title;
                }
            }
            catch (Exception)
            {
                Reset();
            }
        }

        public void Reset()
        {
            Cover = new BitmapImage(new Uri("/BassPlayer2;component/Style/unknown.png", UriKind.Relative));
            Year = "";
            Album = "";
            Title = "";
            Artist = "";
        }
    }
}
