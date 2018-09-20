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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TCPlayer.Code;
using TCPlayer.Properties;
using TaskRunner;
using TCPlayer.Jobs;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        private readonly static DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(PlayList), new PropertyMetadata(0));

        private readonly static DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(PlayList), new PropertyMetadata(0));

        private ObservableCollection<string> _list;

        private void AddDir_Click(object sender, RoutedEventArgs e)
        {
            string[] filters = App.Formats.Split(';');
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = Properties.Resources.Playlist_AddFolderDescription;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> Files = new List<string>(30);
                foreach (var filter in filters)
                {
                    Files.AddRange(Directory.GetFiles(fbd.SelectedPath, filter));
                }
                Files.Sort();
                _list.AddRange(Files);
            }
        }

        private void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Audio Files|" + App.Formats;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _list.AddRange(ofd.FileNames);
            }
        }

        private async void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Playlists | " + App.Playlists;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await LoadPlaylist(ofd.FileName);
            }
        }

        private void AddURL_Click(object sender, RoutedEventArgs e)
        {
            var urld = new AddURLDialog();
            urld.OkClicked = new Action(() =>
            {
                if (!string.IsNullOrEmpty(urld.Url))
                    _list.Add(urld.Url);
            });
            MainWindow.ShowDialog(urld);
        }

        private void DiscMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            DiscMenu.Items.Clear();
            var query = (from drive in DriveInfo.GetDrives()
                         where 
                            drive.DriveType == DriveType.CDRom || 
                            drive.DriveType == DriveType.Removable
                            && drive.IsReady
                         select drive).ToList();

            if (query.Count < 1)
            {
                MenuItem drive = new MenuItem();
                drive.Header = Properties.Resources.Playlist_NoDiscsFound;
                DiscMenu.Items.Add(drive);
            }

            foreach (var drive in query)
            {
                MenuItem device = new MenuItem
                {
                    Header = drive,
                    Tag = drive.DriveType
                };
                device.Click += Drive_Click;
                DiscMenu.Items.Add(device);
            }
        }

        private async void Drive_Click(object sender, RoutedEventArgs e)
        {
            var menuitem = sender as MenuItem;
            var drive = menuitem.Header.ToString();
            var type = DriveType.Removable;

            Enum.TryParse(menuitem.Tag.ToString(), out type);

            var config = new JobRunnerConfiguration<string, IEnumerable<string>>
            {
                JobDescription = "Searching Device for playable content",
                JobTitle = "Loading...",
                ReportTaskBarProgress = true,
                JobInput = drive,
            };

            try
            {
                JobResult<IEnumerable<string>> results;
                if (type == DriveType.CDRom)
                {
                    config.Job = new CDGetInfoJob();
                    results = await JobRunner.RunJob(config);
                    if (results.Result.Any())
                    {
                        _list.AddRange(results.Result);
                    }
                }
                config.Job = new ImportFromMassStorageJob();
                results = await JobRunner.RunJob(config);
                if (results.Result.Any())
                {
                    _list.AddRange(results.Result);
                }

            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, Properties.Resources.Error_FileLoad);
            }
        }

        private void ITunesMenu_FilesProvidedEvent(object sender, IEnumerable<string> e)
        {
            _list.AddRange(e);
        }

        private async Task LoadPlaylist(string filename)
        {
            var config = new JobRunnerConfiguration<string, IEnumerable<string>>
            {
                JobDescription = "Reading playlist file",
                JobTitle = "Loading...",
                ReportTaskBarProgress = true,
                JobInput = filename,
            };

            string ext = Path.GetExtension(filename);
            switch (ext)
            {
                case ".pls":
                    config.Job = new LoadPLSJob();
                    break;
                case ".m3u":
                    config.Job = new LoadM3UJob();
                    break;
                case ".wpl":
                    config.Job = new LoadWPLJob();
                    break;
                case ".asx":
                    config.Job = new LoadASXJob();
                    break;
            }
            try
            {
                var result = await JobRunner.RunJob(config);
                _list.AddRange(result.Result);
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, Properties.Resources.Error_FileLoad);
            }
        }
        private void ManageClear_Click(object sender, RoutedEventArgs e)
        {
            _list.Clear();
        }

        private void ManageDelete_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistView.SelectedItems.Count == 0) return;
            while (PlaylistView.SelectedItems.Count > 0)
            {
                _list.Remove((string)PlaylistView.SelectedItems[0]);
            }
        }

        private void ManageSave_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "M3U list | *.m3u";
            sfd.FilterIndex = 0;
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var targetdir = Path.GetDirectoryName(sfd.FileName);
                using (var contents = File.CreateText(sfd.FileName))
                {
                    foreach (var entry in _list)
                    {
                        var edir = Path.GetDirectoryName(entry);
                        if (edir.StartsWith(targetdir))
                        {
                            var line = entry.Replace(targetdir + "\\", "");
                            contents.WriteLine(line);
                        }
                        else contents.WriteLine(entry);
                    }
                }
            }
        }

        private void PlaylistView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ItemDoubleClcik != null)
            {
                var index = PlaylistView.SelectedIndex;
                if (index < 0) return;
                ItemDoubleClcik(sender, null);
                Index = index + 1;
            }
        }

        private void SortAZ_Click(object sender, RoutedEventArgs e)
        {
            var q = from i in _list orderby i ascending select i;
            var sorted = q.ToList();
            _list.Clear();
            _list.AddRange(sorted);
        }

        private void SortRandom_Click(object sender, RoutedEventArgs e)
        {
            var q = from i in _list orderby Guid.NewGuid() select i;
            var sorted = q.ToList();
            _list.Clear();
            _list.AddRange(sorted);
        }

        private void SortZA_Click(object sender, RoutedEventArgs e)
        {
            var q = from i in _list orderby i descending select i;
            var sorted = q.ToList();
            _list.Clear();
            _list.AddRange(sorted);
        }

        public event RoutedEventHandler ItemDoubleClcik;

        public PlayList()
        {
            InitializeComponent();
            _list = new ObservableCollection<string>();
            _list.CollectionChanged += (s, e) => { Count = _list.Count; };
            PlaylistView.ItemsSource = _list;
        }

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public string SelectedItem
        {
            get
            {
                if (PlaylistView.SelectedIndex == -1) return null;
                return _list[PlaylistView.SelectedIndex];
            }
        }

        public bool CanDoNextTrack()
        {
            return (PlaylistView.SelectedIndex + 1) <= (_list.Count - 1);
        }

        public async void DoLoad(IEnumerable<string> items)
        {
            if (items == null)
                items = Environment.GetCommandLineArgs();

            foreach (var item in items)
            {
                var ext = Path.GetExtension(item).ToLower();
                if (ext.Contains("?") || ext.Contains("&"))
                {
                    var query = new Uri(item);
                    ext = Path.GetExtension(query.AbsolutePath).ToLower();
                }

                if (!string.IsNullOrEmpty(ext) && App.Playlists.Contains(ext))
                {
                    await LoadPlaylist(item);
                }
                else if (App.Formats.Contains(ext))
                {
                    _list.Add(item);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Playlist_UnsupportedListFormat,
                                    Properties.Resources.Playlist_UnsupportedListFormatTitle,
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void NextTrack()
        {
            if (PlaylistView.SelectedIndex + 1 < _list.Count)
            {
                PlaylistView.SelectedIndex += 1;
                Index = PlaylistView.SelectedIndex + 1;
            }
        }

        public void PreviousTrack()
        {
            Dispatcher.Invoke(() =>
            {
                if (PlaylistView.SelectedIndex - 1 > -1)
                {
                    PlaylistView.SelectedIndex -= 1;
                    Index = PlaylistView.SelectedIndex + 1;
                }
            });
        }
    }
}
