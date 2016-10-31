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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TCPlayer.Code;
using ManagedBass;
using System.Runtime.InteropServices;

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
            DependencyProperty.Register("Cover", typeof(ImageSource), typeof(SongData), new PropertyMetadata(new BitmapImage(new Uri("pack://application:,,,/TCPlayer;component/Style/unknown.png"))));

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

        public void UpdateMediaInfo(string file, int handle)
        {
            var fi = new FileInfo(file);
            FileName = fi.Name;
            Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/music.png", UriKind.Relative));
            Size = GetFileSize(fi.Length);
            Artist = Marshal.PtrToStringAuto(Bass.ChannelGetTags(handle, TagType.MusicAuth));
            Title = Marshal.PtrToStringAuto(Bass.ChannelGetTags(handle, TagType.MusicName));
            Album = "";
            Year = "unknown";
        }

        public void UpdateMediaInfo(string file)
        {
            var notify = Properties.Settings.Default.TrackChangeNotification;
            if (file.StartsWith("http://") || file.StartsWith("https://"))
            {
                FileName = file;
                Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/network.png", UriKind.Relative));
                Year = DateTime.Now.Year.ToString();
                Album = "";
                Title = "Stream";
                Artist = Path.GetFileName(file);
                Size = "Infinite";
                if (notify) App._notify.ShowNotification(file);
                return;
            }
            else if (file.StartsWith("cd://"))
            {
                string[] info = file.Replace("cd://", "").Split('/');
                var drive = Convert.ToInt32(info[0]);
                var track = Convert.ToInt32(info[1]);

                var size = ManagedBass.Cd.BassCd.GetTrackLength(drive, track);
                Size = GetFileSize(size);
                UpdateCDFlags(track + 1, notify);
                return;
            }
            try
            {
                var fi = new FileInfo(file);
                FileName = fi.Name;
                Size = GetFileSize(fi.Length);

                if (Helpers.IsMidi(file))
                {
                    Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/midi.png", UriKind.Relative));
                    Album = "";
                    Title = fi.Name;
                    Year = "Unknown";
                    Artist = "";
                    if (notify) App._notify.ShowNotification(FileName);
                    return;
                }

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
                }
                Year = tags.Tag.Year.ToString();
                Artist = tags.Tag.Performers[0];
                Album = tags.Tag.Album;
                Title = tags.Tag.Title;
                if (notify) App._notify.ShowNotification(FileName, Artist, Title);
            }
            catch (Exception)
            {
                Reset();
            }
        }

        private void UpdateCDFlags(int track, bool notify)
        {
            FileName = string.Format("CD Track #{0}", track);
            Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/disk.png", UriKind.Relative));
            Year = "unknown";
            if (App._cddata.Count > 0)
            {
                Artist = App._cddata[string.Format("PERFORMER{0}", track)];
                Title = App._cddata[string.Format("TITLE{0}", track)];
                Album = App._cddata["TITLE0"];
            }
            else
            {
                Artist = "Track";
                Title = string.Format("#{0}", track);
                Album = "Audio CD";
            }
            if (notify)
                App._notify.ShowNotification("CD Track" + track, Artist, Title);
        }

        public void Reset()
        {
            Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/unknown.png", UriKind.Relative));
            Year = "";
            Album = "";
            Title = "";
            Artist = "";
        }
    }
}
