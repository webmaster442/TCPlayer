using AppLib.WPF.MVVM;
using System;
using System.Collections.ObjectModel;
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

        private void ExecuteAddFolder()
        {
            throw new NotImplementedException();
        }

        private void ExecuteAddFiles()
        {
            throw new NotImplementedException();
        }
    }
}
