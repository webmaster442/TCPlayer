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
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TCPlayer.MediaLibary.DB;
using AppLib.Common.MessageHandler;

namespace TCPlayer
{
    public interface IMediaWindowView : IView
    {
        bool IsProgressVisible { get; set; }
    }

    public class MediaWindowViewModel : ViewModel<IMediaWindowView>
    {
        public DelegateCommand ManageAddFilesCommand { get; private set; }
        public DelegateCommand ManageAddFolderCommand { get; private set; }
        public DelegateCommand PlaySelectedTrackCommand { get; private set; }

        public ObservableCollection<TrackEntity> DisplayItems { get; private set; }
        public TrackEntity SelectedTrack { get; set; }

        public MediaWindowViewModel()
        {
            DisplayItems = new ObservableCollection<TrackEntity>();
            ManageAddFilesCommand = DelegateCommand.ToCommand(ExecuteAddFiles);
            ManageAddFolderCommand = DelegateCommand.ToCommand(ExecuteAddFolder);
            PlaySelectedTrackCommand = DelegateCommand.ToCommand(ExecutePlaySelectedTrackCommand);
        }

        private void ExecutePlaySelectedTrackCommand()
        {
            if (SelectedTrack != null)
            {
                Messager.Instance.SendMessage(typeof(MainWindow), SelectedTrack.Path);
                DataBase.Instance.UpdateTrackPlayInfo(SelectedTrack);
            }
        }

        private async void ExecuteAddFolder()
        {
            string[] filters = App.Formats.Split(';');
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = Properties.Resources.Playlist_AddFolderDescription;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                View.IsProgressVisible = true;
                List<string> Files = new List<string>(30);
                foreach (var filter in filters)
                {
                    Files.AddRange(Directory.GetFiles(fbd.SelectedPath, filter));
                }
                Files.Sort();
                await DataBase.Instance.AddFiles(Files);
                View.IsProgressVisible = false;
            }
        }

        private async void ExecuteAddFiles()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Audio Files|" + App.Formats;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                View.IsProgressVisible = true;
                await DataBase.Instance.AddFiles(ofd.FileNames);
                View.IsProgressVisible = false;
            }
        }

        public void DoQuery(QueryInput queryInput)
        {
            var items = DataBase.Instance.Execute(queryInput);
            DisplayItems.Clear();
            DisplayItems.AddRange(items);
        }
    }
}
