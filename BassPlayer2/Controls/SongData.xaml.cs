using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BassPlayer2.Controls
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

        public static DependencyProperty FormatSizeProperty =
            DependencyProperty.Register("FormatSize", typeof(string), typeof(SongData));

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

        public string FormatSize
        {
            get { return (string)GetValue(FormatSizeProperty); }
            set { SetValue(FormatSizeProperty, value); }
        }
    }
}
