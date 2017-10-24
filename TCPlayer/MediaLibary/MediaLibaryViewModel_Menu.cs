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
using AppLib.Common.MessageHandler;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using TCPlayer.MediaLibary.DB;
using TCPlayer.Properties;

namespace TCPlayer.MediaLibary
{
    public partial class MediaLibaryViewModel : ViewModel<IMediaLibaryView>
    {
        private async void MenuAddFiles()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|" + App.Formats
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await DataBase.Instance.AddFiles(ofd.FileNames);
            }
        }

        private async void MenuAddFolder()
        {
            string[] filters = App.Formats.Split(';');
            var fbd = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = Resources.Playlist_AddFolderDescription
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> Files = new List<string>(30);
                foreach (var filter in filters)
                {
                    Files.AddRange(Directory.GetFiles(fbd.SelectedPath, filter));
                }
                Files.Sort();
                await DataBase.Instance.AddFiles(Files);
            }
        }

        private void MenuSendToPlaylist()
        {
            Messager.Instance.SendMessage(SelectedItems);
        }

        private void MenuCreateQuery()
        {
            var queryeditor = new QueryEditor();
            View.ShowDialog(queryeditor);
        }

        private void MenuBackupDb()
        {
            var sfd = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "TC Player Media database|TCPlayerDb.db",
                FileName = "TCPlayerDb.db"
            };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.Copy(DataBase.Instance.DataBaseFileLocation, sfd.FileName);
                MessageBox.Show("Backup succesfull", "Database backup", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
