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
using ManagedBass;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TCPlayer.Code;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for SongData.xaml
    /// </summary>
    public partial class SongData : UserControl
    {
        Player _player;

        public SongData()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _player = Player.Instance;
            _player.MetaChanged += _player_MetaChanged;
            Spectrum.RegisterSoundPlayer(_player);
        }

        public static DependencyProperty CoverProperty =
            DependencyProperty.Register("Cover", typeof(ImageSource), typeof(SongData), new PropertyMetadata(new BitmapImage(new Uri("pack://application:,,,/TCPlayer;component/Style/unknown.png"))));

        public static DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(SongData));

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

        public int Handle { get; set; }

        private void SetInfoText(string artist, string title, string album, string year, string size)
        {
            artist = string.IsNullOrEmpty(artist) ? "Unknown Artist" : artist;
            title = string.IsNullOrEmpty(title) ? "Unknown Song" : title;
            year = string.IsNullOrEmpty(year) ? DateTime.Now.Year.ToString() : year;

            var sb = new StringBuilder();
            sb.AppendFormat("{0} - {1}\r\n", artist, title);
            sb.AppendFormat("{0} ({1})\r\n", album, year);
            sb.Append(size);
            InfoText.Text = sb.ToString();
        }

        private void SetInfoText(string artisttitle, string album, string year, string size)
        {
            artisttitle = string.IsNullOrEmpty(artisttitle) ? "Unknown Artist - Unknown Song" : artisttitle;
            year = string.IsNullOrEmpty(year) ? DateTime.Now.Year.ToString() : year;

            var sb = new StringBuilder();
            sb.AppendFormat("{0}\r\n", artisttitle);
            sb.AppendFormat("{0} ({1})\r\n", album, year);
            sb.Append(size);
            InfoText.Text = sb.ToString();
        }

        public void UpdateMediaInfo(string file, int handle)
        {
            FileName = file;
            var fi = new FileInfo(file);
            Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/music.png", UriKind.Relative));
            var Size = GetFileSize(fi.Length);
            var Artist = Marshal.PtrToStringAuto(Bass.ChannelGetTags(handle, TagType.MusicAuth));
            var Title = Marshal.PtrToStringAuto(Bass.ChannelGetTags(handle, TagType.MusicName));
            SetInfoText(Artist, Title, "", "unknown", Size);
        }

        private void _player_MetaChanged(object sender, string e)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.Invoke(() => { SetInfoText(e, FileName, DateTime.Now.Year.ToString(), "stream"); });
        }

        public void UpdateMediaInfo(string file)
        {
            FileName = file;
            var notify = Properties.Settings.Default.TrackChangeNotification;
            if (file.StartsWith("http://") || file.StartsWith("https://"))
            {
                Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/network.png", UriKind.Relative));
                SetInfoText(Path.GetFileName(file), "Stream", "", DateTime.Now.Year.ToString(), "stream");
                if (notify) App.NotifyIcon.ShowNotification(file);
                return;
            }
            else if (file.StartsWith("cd://"))
            {
                string[] info = file.Replace("cd://", "").Split('/');
                var drive = Convert.ToInt32(info[0]);
                var track = Convert.ToInt32(info[1]);
                var size = ManagedBass.Cd.BassCd.GetTrackLength(drive, track);
                UpdateCDFlags(track + 1, notify, size);
                return;
            }
            try
            {
                var fi = new FileInfo(file);
                var Size = GetFileSize(fi.Length);
                if (Helpers.IsMidi(file))
                {
                    Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/midi.png", UriKind.Relative));
                    SetInfoText("", fi.Name, "", "Unknown", Size);
                    if (notify) App.NotifyIcon.ShowNotification(FileName);
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
                var Year = tags.Tag.Year.ToString();
                var Artist = "";
                if (tags.Tag.Performers != null && tags.Tag.Performers.Length != 0) Artist = tags.Tag.Performers[0];
                var Album = tags.Tag.Album;
                var Title = tags.Tag.Title;
                if (notify) App.NotifyIcon.ShowNotification(FileName, Artist, Title);
                SetInfoText(Artist, Title, Album, Year, Size);
            }
            catch (Exception)
            {
                Reset();
            }
        }

        private void UpdateCDFlags(int track, bool notify, int size)
        {
            FileName = string.Format("CD Track #{0}", track);
            //GetFileSize(size);
            Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/disk.png", UriKind.Relative));
            var Year = "unknown";
            var Artist = "Track";
            var Title = string.Format("#{0}", track);
            var Album = "Audio CD";
            if (App.CdData.Count > 0)
            {
                Artist = App.CdData[string.Format("PERFORMER{0}", track)];
                Title = App.CdData[string.Format("TITLE{0}", track)];
                Album = App.CdData["TITLE0"];
            }
            if (notify)
                App.NotifyIcon.ShowNotification("CD Track" + track, Artist, Title);
            SetInfoText(Artist, Title, Album, Year, GetFileSize(size));
        }

        public void Reset()
        {
            Cover = new BitmapImage(new Uri("/TCPlayer;component/Style/unknown.png", UriKind.Relative));
            InfoText.Text = "Tag read error";
        }
    }
}
