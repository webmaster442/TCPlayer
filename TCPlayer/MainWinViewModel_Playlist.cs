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
using AppLib.Common.Extensions;
using AppLib.Common.MessageHandler;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using TCPlayer.Code;
using TCPlayer.Controls;
using TCPlayer.MediaLibary.DB;

namespace TCPlayer
{
    public partial class MainWinViewModel : ViewModel<IMainWindow>, IMessageClient<IEnumerable<TrackEntity>>
    {
        public async void DoLoad(IEnumerable<string> items)
        {
            View?.SetPage(TabPage.PlayList);
            if (items == null)
                items = Environment.GetCommandLineArgs();

            foreach (var item in items)
            {
                var ext = Path.GetExtension(item).ToLower();
                if (!string.IsNullOrEmpty(ext) && App.Playlists.Contains(ext))
                {
                    string[] result = null;
                    switch (ext)
                    {
                        case ".pls":
                            result = await PlaylistLoaders.LoadPls(item);
                            break;
                        case ".m3u":
                            result = await PlaylistLoaders.LoadM3u(item);
                            break;
                        case ".wpl":
                            result = await PlaylistLoaders.LoadWPL(item);
                            break;
                        case ".asx":
                            result = await PlaylistLoaders.LoadASX(item);
                            break;
                    }
                    PlayList.AddRange(result);
                }
                else if (App.Formats.Contains(ext))
                {
                    PlayList.Add(item);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Playlist_UnsupportedListFormat,
                                    Properties.Resources.Playlist_UnsupportedListFormatTitle,
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PlaylistDelete()
        {
            View?.SetPage(TabPage.PlayList);
            View?.DeleteSelectedFromPlaylist();
        }

        private void PlaylistClear()
        {
            View?.SetPage(TabPage.PlayList);
            PlayList.Clear();
            PlayListIndex = -1;
        }

        private void PlaylistSave()
        {
            View?.SetPage(TabPage.PlayList);
            var sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "M3U list | *.m3u";
            sfd.FilterIndex = 0;
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var targetdir = Path.GetDirectoryName(sfd.FileName);
                using (var contents = File.CreateText(sfd.FileName))
                {
                    foreach (var entry in PlayList)
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

        private void PlaylistAddDirectory()
        {
            View?.SetPage(TabPage.PlayList);
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
                PlayList.AddRange(Files);
            }
        }

        private void PlaylistAddFile()
        {
            View?.SetPage(TabPage.PlayList);
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Audio Files|" + App.Formats;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PlayList.AddRange(ofd.FileNames);
            }
        }

        private async void PlaylistAddPlaylist()
        {
            View?.SetPage(TabPage.PlayList);
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Playlists | " + App.Playlists;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName);
                string[] result = null;
                switch (ext)
                {
                    case ".pls":
                        result = await PlaylistLoaders.LoadPls(ofd.FileName);
                        break;
                    case ".m3u":
                        result = await PlaylistLoaders.LoadM3u(ofd.FileName);
                        break;
                    case ".wpl":
                        result = await PlaylistLoaders.LoadWPL(ofd.FileName);
                        break;
                    case ".asx":
                        result = await PlaylistLoaders.LoadASX(ofd.FileName);
                        break;
                }
                PlayList.AddRange(result);
            }
        }

        private void PlaylistAddUrl()
        {
            View?.SetPage(TabPage.PlayList);
            var urld = new AddURLDialog();
            urld.OkClicked = new Action(() =>
            {
                if (!string.IsNullOrEmpty(urld.Url))
                    PlayList.Add(urld.Url);
            });
        }

        private void PlaylistSortAsc()
        {
            View?.SetPage(TabPage.PlayList);
            var q = from i in PlayList.Clone() orderby i ascending select i;
            var sorted = q.ToList();
            PlayList.Clear();
            PlayList.AddRange(sorted);
        }

        private void PlaylistSortDesc()
        {
            View?.SetPage(TabPage.PlayList);
            var q = from i in PlayList.Clone() orderby i descending select i;
            PlayList.Clear();
            PlayList.AddRange(q);
        }

        private void PlaylistSortRandom()
        {
            View?.SetPage(TabPage.PlayList);
            var q = from i in PlayList.Clone() orderby Guid.NewGuid() select i;
            PlayList.Clear();
            PlayList.AddRange(q);
        }

        private void PlaylistSortDisticnt()
        {
            View?.SetPage(TabPage.PlayList);
            var q = PlayList.Clone().Distinct();
            PlayList.Clear();
            PlayList.AddRange(q);
        }

        public void HandleMessage(IEnumerable<TrackEntity> message)
        {
            var filenames = message.Select(e => e.Path);
            PlayList.AddRange(filenames);
            View.SetPage(TabPage.PlayList);
        }
    }
}
