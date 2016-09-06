using TCPlayer.Code;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        private ObservableCollection<string> _list;

        public PlayList()
        {
            InitializeComponent();
            _list = new ObservableCollection<string>();
            PlaylistView.ItemsSource = _list;
        }

        public event RoutedEventHandler ItemDoubleClcik;

        public string SelectedItem
        {
            get
            {
                if (PlaylistView.SelectedIndex == -1) return null;
                return _list[PlaylistView.SelectedIndex];
            }
        }

        public void NextTrack()
        {
            Dispatcher.Invoke(() =>
            {
                if (PlaylistView.SelectedIndex + 1 < _list.Count)
                    PlaylistView.SelectedIndex += 1;
            });
        }

        public void PreviousTrack()
        {
            Dispatcher.Invoke(() =>
            {
                if (PlaylistView.SelectedIndex - 1 > -1)
                    PlaylistView.SelectedIndex -= 1;
            });
        }

        public void DoLoad(IEnumerable<string> items)
        {
            if (items == null)
                items = Environment.GetCommandLineArgs();

            foreach (var item in items)
            {
                var ext = Path.GetExtension(item);
                if (App.Playlists.Contains(ext))
                {
                    string[] result = null;
                    switch (ext)
                    {
                        case ".pls":
                            result = PlaylistLoaders.LoadPls(item);
                            break;
                        case ".m3u":
                            result = PlaylistLoaders.LoadM3u(item);
                            break;
                        case ".wpl":
                            result = PlaylistLoaders.LoadWPL(item);
                            break;
                    }
                    _list.AddRange(result);
                }
                else if (App.Formats.Contains(ext))
                {
                    _list.Add(item);
                }
            }
        }

        private void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Playlists | " + App.Playlists;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName);
                string[] result = null;
                switch (ext)
                {
                    case ".pls":
                        result = PlaylistLoaders.LoadPls(ofd.FileName);
                        break;
                    case ".m3u":
                        result = PlaylistLoaders.LoadM3u(ofd.FileName);
                        break;
                    case ".wpl":
                        result = PlaylistLoaders.LoadWPL(ofd.FileName);
                        break;
                }
                _list.AddRange(result);
            }
        }

        private void AddDir_Click(object sender, RoutedEventArgs e)
        {
            string[] filters = App.Formats.Split(';');
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "Select folder to be added";
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

        private void SortAZ_Click(object sender, RoutedEventArgs e)
        {
            var q = from i in _list orderby i ascending select i;
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

        private void SortRandom_Click(object sender, RoutedEventArgs e)
        {
            var q = from i in _list orderby Guid.NewGuid() select i;
            var sorted = q.ToList();
            _list.Clear();
            _list.AddRange(sorted);
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
            }
        }
    }
}
