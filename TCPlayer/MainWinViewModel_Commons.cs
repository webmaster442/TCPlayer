using AppLib.WPF.MVVM;
using System;
using System.Collections.ObjectModel;
using TCPlayer.Code;
using TCPlayer.Controls;

namespace TCPlayer
{
    public partial class MainWinViewModel: ViewModel<IMainWindow>
    {
        private int _playlistindex;

        //General Player commands
        public DelegateCommand PlayerAboutCommand { get; private set; }
        public DelegateCommand PlayerSettingsCommand { get; private set; }
        public DelegateCommand PlayerExitCommand { get; private set; }
        public DelegateCommand PlayerChangeOutputCommand { get; private set; }
        //PlaylistCommands
        public DelegateCommand PlaylistSaveCommand { get; private set; }
        public DelegateCommand PlaylistAddDirectoryCommnad { get; private set; }
        public DelegateCommand PlaylistAddFileCommand { get; private set; }
        public DelegateCommand PlaylistAddPlaylistCommand { get; private set; }
        public DelegateCommand PlaylistAddUrlCommand { get; private set; }
        public DelegateCommand PlaylistSortAscCommand { get; private set; }
        public DelegateCommand PlaylistSortDescCommand { get; private set; }
        public DelegateCommand PlaylistSortRandomCommand { get; private set; }
        public DelegateCommand PlaylistSortDisticntCommand { get; private set; }
        public DelegateCommand PlaylistClearCommand { get; private set; }
        public DelegateCommand PlaylistDeleteCommand { get; private set; }

        public DelegateCommand FileExplorerNavigateHomeCommand { get; private set; }
        public DelegateCommand FileExplorerPlaySelectedCommand { get; private set; }
        public DelegateCommand FileExplorerAddToPlaylistCommand { get; private set; }

        public ObservableCollection<string> PlayList { get; private set; }

        public MediaLibary.MediaLibaryViewModel MediaViewModel { get; private set; }

        public int PlayListIndex
        {
            get { return _playlistindex; }
            set
            {
                SetValue(ref _playlistindex, value);
                NotifyPropertyChanged("CorrectedPlayListIndex");
            }
        }

        public int CorrectedPlayListIndex
        {
            get
            {
                if (_playlistindex < 0 && PlayList.Count == 0)
                    return 0;
                else
                    return _playlistindex + 1;
            }
        }

        public MainWinViewModel()
        {
            //General Player commands
            PlayerAboutCommand = DelegateCommand.ToCommand(PlayerAbout);
            PlayerSettingsCommand = DelegateCommand.ToCommand(PlayerSettings);
            PlayerExitCommand = DelegateCommand.ToCommand(PlayerExit);
            PlayerChangeOutputCommand = DelegateCommand.ToCommand(PlayerChangeOutput);

            //Playlist commands
            PlayList = new ObservableCollection<string>();
            PlayListIndex = -1;
            PlaylistSaveCommand = DelegateCommand.ToCommand(PlaylistSave);
            PlaylistAddDirectoryCommnad = DelegateCommand.ToCommand(PlaylistAddDirectory);
            PlaylistAddFileCommand = DelegateCommand.ToCommand(PlaylistAddFile);
            PlaylistAddPlaylistCommand = DelegateCommand.ToCommand(PlaylistAddPlaylist);
            PlaylistAddUrlCommand = DelegateCommand.ToCommand(PlaylistAddUrl);
            PlaylistSortAscCommand = DelegateCommand.ToCommand(PlaylistSortAsc);
            PlaylistSortDescCommand = DelegateCommand.ToCommand(PlaylistSortDesc);
            PlaylistSortRandomCommand = DelegateCommand.ToCommand(PlaylistSortRandom);
            PlaylistSortDisticntCommand = DelegateCommand.ToCommand(PlaylistSortDisticnt);
            PlaylistClearCommand = DelegateCommand.ToCommand(PlaylistClear);
            PlaylistDeleteCommand = DelegateCommand.ToCommand(PlaylistDelete);

            FileExplorerNavigateHomeCommand = DelegateCommand.ToCommand(FileExplorerNavigateHome);
            FileExplorerAddToPlaylistCommand = DelegateCommand.ToCommand(FileExplorerAddToPlaylist);
            FileExplorerPlaySelectedCommand = DelegateCommand.ToCommand(FileExplorerPlaySelected);

            MediaViewModel = new MediaLibary.MediaLibaryViewModel();
        }

        private void PlayerExit()
        {
            App.Current.MainWindow.Close();
        }

        private void PlayerSettings()
        {
            var settings = new Settings();
            View.ShowDialog(settings);
        }

        private void PlayerAbout()
        {
            var about = new AboutDialog();
            View.ShowDialog(about);
        }

        private void PlayerChangeOutput()
        {
            var selector = new DeviceChange();
            string[] devices = Player.Instance.GetDevices();
            selector.DataContext = devices;
            selector.OkClicked = new Action(() =>
            {
                var name = devices[selector.DeviceIndex];
                Properties.Settings.Default.SampleRate = selector.SampleRate;
                Player.Instance.ChangeDevice(name);
            });
            View.ShowDialog(selector);
        }
    }
}
