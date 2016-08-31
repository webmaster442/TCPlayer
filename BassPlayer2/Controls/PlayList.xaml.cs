using BassPlayer2.Code;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Linq;

namespace BassPlayer2.Controls
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

        private void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Playlists | " + App.Playlists;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = Path.GetFileName(ofd.FileName);
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
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
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
            while (PlaylistView.SelectedItems != null)
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
    }
}
