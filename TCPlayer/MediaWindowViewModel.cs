using AppLib.Common.Extensions;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TCPlayer.MediaLibary.DB;

namespace TCPlayer
{
    public class MediaWindowViewModel: BindableBase
    {
        public DelegateCommand ManageAddFilesCommand { get; private set; }
        public DelegateCommand ManageAddFolderCommand { get; private set; }

        public ObservableCollection<TrackEntity> DisplayItems { get; private set; }

        public MediaWindowViewModel()
        {
            DisplayItems = new ObservableCollection<TrackEntity>();
            ManageAddFilesCommand = DelegateCommand.ToCommand(ExecuteAddFiles);
            ManageAddFolderCommand = DelegateCommand.ToCommand(ExecuteAddFolder);
        }

        private async void ExecuteAddFolder()
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
                await DataBase.Instance.AddFiles(Files);
            }
        }

        private async void ExecuteAddFiles()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Audio Files|" + App.Formats;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await DataBase.Instance.AddFiles(ofd.FileNames);
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
