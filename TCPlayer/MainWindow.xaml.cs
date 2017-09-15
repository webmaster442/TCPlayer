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
using AppLib.Common;
using AppLib.Common.Extensions;
using AppLib.Common.MessageHandler;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Threading;
using TCPlayer.Code;
using TCPlayer.Controls;

namespace TCPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable, IMainWindow,  IMessageClient<string>
    {
        private float _prevvol;
        private DispatcherTimer _SongTimer;
        private bool _loaded;
        private bool _isdrag;
        private KeyboardHook _keyboardhook;
        private ChapterProvider _chapterprovider;

        public MainWinViewModel ViewModel
        {
            get;
            private set;
        }

        public UId MessageReciverID
        {
            get { return new UId(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            Messager.Instance.SubScribe(this);
            Player.Instance.ChangeDevice(); //init
            _prevvol = 1.0f;
            _chapterprovider = new ChapterProvider(ChapterMenu);
            _chapterprovider.ChapterClicked += _chapterprovider_ChapterClicked;
            _SongTimer = new DispatcherTimer();
            _SongTimer.Interval = TimeSpan.FromMilliseconds(40);
            _SongTimer.IsEnabled = false;
            _SongTimer.Tick += _SongTimerTick;
            _loaded = true;
            ViewModel = this.GetViewModel<MainWinViewModel>();

            if (Player.Instance.Is64Bit) Title += " (x64)";
            else Title += " (x86)";

            var src = Environment.GetCommandLineArgs();
            string[] pars = new string[src.Length - 1];
            Array.Copy(src, 1, pars, 0, src.Length - 1);
            DoLoadAndPlay(pars);

            if (Properties.Settings.Default.SaveVolume)
            {
                var vol = Properties.Settings.Default.LastVolume;
                if (vol > -1) VolSlider.Value = vol;
            }

            if (Properties.Settings.Default.RegisterMultimediaKeys)
                RegisterMultimedaKeys();

            if (Properties.Settings.Default.TrackChangeNotification)
                App.NotifyIcon = new NotificationIcon();

            this.GetViewModel<MainWinViewModel>().View = this;
        }

        public void ShowDialog(UserControl dialog)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main.OverLayContent.Children.Clear();
            main.OverLayContent.Children.Add(dialog);
            main.OverLay.Visibility = Visibility.Visible;
        }

        private void OverLayClose_Click(object sender, RoutedEventArgs e)
        {
            OverLay.Visibility = Visibility.Collapsed;
            OverLayContent.Children.Clear();
        }

        private void OverLayOk_Click(object sender, RoutedEventArgs e)
        {
            OverLay.Visibility = Visibility.Collapsed;
            var dialog = (OverLayContent.Children[0] as IDialog);
            dialog?.OkClicked?.Invoke();
            OverLayContent.Children.Clear();
        }

        protected virtual void Dispose(bool native)
        {
            if (Player.Instance != null)
            {
                Player.Instance.Dispose();
            }
            if (_keyboardhook != null)
            {
                _keyboardhook.Dispose();
                _keyboardhook = null;
            }
            if (App.NotifyIcon != null)
            {
                App.NotifyIcon.Dispose();
                App.NotifyIcon = null;
            }
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void HandleMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                DoLoadAndPlay(new string[] { message });
            }
        }

        public bool CanDoNexPlaylisttTrack()
        {
            return (PlaylistView.SelectedIndex + 1) <= (ViewModel.PlayList.Count - 1);
        }

        public void NextPlaylistTrack()
        {
            if (PlaylistView.SelectedIndex + 1 < ViewModel.PlayList.Count)
            {
                PlaylistView.SelectedIndex += 1;
                ViewModel.PlayListIndex = PlaylistView.SelectedIndex + 1;
            }
        }

        public void PreviousPlaylistTrack()
        {
            Dispatcher.Invoke(() =>
            {
                if (PlaylistView.SelectedIndex - 1 > -1)
                {
                    PlaylistView.SelectedIndex -= 1;
                    ViewModel.PlayListIndex = PlaylistView.SelectedIndex + 1;
                }
            });
        }

        public void DoLoadAndPlay(IEnumerable<string> items)
        {
            var needplay = false;
            if (ViewModel.PlayList.Count < 1) needplay = true;
            this.GetViewModel<MainWinViewModel>().DoLoad(items);
            if (needplay)
            {
                NextPlaylistTrack();
                StartPlay();
            }
            else
                SetPage(TabPage.PlayList);
            DoBringIntoView();
        }

        private void DoBringIntoView()
        {
            if (!Topmost)
            {
                WindowState = WindowState.Normal;
                Topmost = true;
                Topmost = false;
                this.Activate();
            }
        }

        private void ResetSongSeekUI()
        {
            _SongTimer.IsEnabled = false;
            SeekSlider.Value = 0;
            Taskbar.ProgressState = TaskbarItemProgressState.Normal;
            Taskbar.ProgressValue = 0;
            TbCurrTime.Text = TimeSpan.FromSeconds(0).ToShortTime();
            TbFullTime.Text = TimeSpan.FromSeconds(0).ToShortTime();
        }


        private string SelectedItem
        {
            get
            {
                if (PlaylistView.SelectedIndex == -1) return null;
                return ViewModel.PlayList[PlaylistView.SelectedIndex];
            }
        }

        public void DeleteSelectedFromPlaylist()
        {
            if (PlaylistView.SelectedItems.Count == 0) return;
            while (PlaylistView.SelectedItems.Count > 0)
            {
                ViewModel.PlayList.Remove((string)PlaylistView.SelectedItems[0]);
            }

        }

        private void StartPlay()
        {
            try
            {
                if (!_loaded || SelectedItem == null) return;
                ResetSongSeekUI();
                SongDat.Reset();
                var file = SelectedItem;
                Player.Instance.Load(file);
                _chapterprovider.Clear();
                if (Player.Instance.IsStream) Taskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
                else
                {
                    _chapterprovider.CreateChapters(file, Player.Instance.Length);
                    var len = TimeSpan.FromSeconds(Player.Instance.Length);
                    TbFullTime.Text = len.ToShortTime();
                    SeekSlider.Maximum = Player.Instance.Length;
                }
                _SongTimer.IsEnabled = true;
                if (Helpers.IsTracker(file)) SongDat.UpdateMediaInfo(file, Player.Instance.SourceHandle);
                else SongDat.UpdateMediaInfo(file);
                SongDat.Handle = Player.Instance.MixerHandle;

            }
            catch (Exception ex)
            {
                _SongTimer.IsEnabled = false;
                SongDat.Handle = 0;
                ResetSongSeekUI();
                SongDat.Reset();
                Helpers.ErrorDialog(ex);
            }
        }

        private void _SongTimerTick(object sender, EventArgs e)
        {
            if (!_loaded) return;

            if ((IsActive || Topmost) && (MainView.SelectedIndex == 0))
            {
                if (!Player.Instance.IsPlaying) Player.Instance.IsPlaying = true;

                if (_isdrag)
                {
                    TbCurrTime.Text = TimeSpan.FromSeconds(SeekSlider.Value).ToShortTime();
                    return;
                }
                var pos = TimeSpan.FromSeconds(Player.Instance.Position);
                TbCurrTime.Text = pos.ToShortTime();
                if (Player.Instance.IsStream) SeekSlider.Value = 0;
                else
                {
                    SeekSlider.Value = Player.Instance.Position;
                    Taskbar.ProgressValue = SeekSlider.Value / SeekSlider.Maximum;
                }
                int l, r;
                Player.Instance.VolumeValues(out l, out r);
                if (l < 0) l *= -1;
                if (r < 0) r *= -1;
                VuR.Value = l;
                VuL.Value = r;
            }
            else
            {
                if (Player.Instance.IsStream) SeekSlider.Value = 0;
                else
                {
                    SeekSlider.Value = Player.Instance.Position;
                    Taskbar.ProgressValue = SeekSlider.Value / SeekSlider.Maximum;
                }
                Player.Instance.IsPlaying = false;
            }
        }

        private void DoAction(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            var btn = sender as Button;
            if (btn == null) return;
            switch (btn.Name)
            {
                case "BtnOpen":
                    DoOpen();
                    break;
                case "BtnPlayPause":
                    Player.Instance.PlayPause();
                    break;
                case "BtnStop":
                    Player.Instance.Stop();
                    break;
                case "BtnSeekBack":
                    _SongTimer.IsEnabled = false;
                    Player.Instance.Position -= 5;
                    _SongTimer.IsEnabled = true;
                    break;
                case "BtnSeekFwd":
                    _SongTimer.IsEnabled = false;
                    Player.Instance.Position += 5;
                    _SongTimer.IsEnabled = true;
                    break;
                case "BtnNextTrack":
                    NextPlaylistTrack();
                    StartPlay();
                    break;
                case "BtnPrevTrack":
                    PreviousPlaylistTrack();
                    StartPlay();
                    break;
            }
            _SongTimer.IsEnabled = !Player.Instance.IsPaused;
            if (Player.Instance.IsPaused) Taskbar.ProgressState = TaskbarItemProgressState.Paused;
            else if (!Player.Instance.IsStream) Taskbar.ProgressState = TaskbarItemProgressState.Normal;
            else Taskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
        }

        private void RegisterMultimedaKeys()
        {
            _keyboardhook = new KeyboardHook();
            _keyboardhook.KeyPressed += _keyboardhook_KeyPressed;
            try
            {
                _keyboardhook.RegisterHotKey(AppLib.Common.ModifierKeys.None, System.Windows.Forms.Keys.MediaPlayPause);
                _keyboardhook.RegisterHotKey(AppLib.Common.ModifierKeys.None, System.Windows.Forms.Keys.MediaStop);
                _keyboardhook.RegisterHotKey(AppLib.Common.ModifierKeys.None, System.Windows.Forms.Keys.MediaNextTrack);
                _keyboardhook.RegisterHotKey(AppLib.Common.ModifierKeys.None, System.Windows.Forms.Keys.MediaPreviousTrack);
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, Properties.Resources.MainWin_ErrorMediaKeys);
            }
        }

        private void _keyboardhook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Forms.Keys.MediaPlayPause:
                    Player.Instance.PlayPause();
                    break;
                case System.Windows.Forms.Keys.MediaPreviousTrack:
                    PreviousPlaylistTrack();
                    StartPlay();
                    break;
                case System.Windows.Forms.Keys.MediaNextTrack:
                    NextPlaylistTrack();
                    StartPlay();
                    break;
                case System.Windows.Forms.Keys.MediaStop:
                    Player.Instance.Stop();
                    break;
            }
        }

        private void ThumbButtonInfo_Click(object o, EventArgs e)
        {
            var param = (o as ThumbButtonInfo).CommandParameter.ToString();
            switch (param)
            {
                case "Play/Pause":
                    Player.Instance.PlayPause();
                    break;
                case "Previous track":
                    PreviousPlaylistTrack();
                    StartPlay();
                    break;
                case "Next track":
                    NextPlaylistTrack();
                    StartPlay();
                    break;
                case "Mute/UnMute":
                    var state = (bool)BtnMute.IsChecked;
                    BtnMute.IsChecked = !state;
                    BtnMute_Click(null, null);
                    break;
            }
            _SongTimer.IsEnabled = !Player.Instance.IsPaused;
            if (Player.Instance.IsPaused) Taskbar.ProgressState = TaskbarItemProgressState.Paused;
            else Taskbar.ProgressState = TaskbarItemProgressState.Normal;
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if (BtnMute.IsChecked == true)
            {
                _prevvol = (float)VolSlider.Value;
                VolSlider.Value = 0;
                VolSlider.IsEnabled = false;
            }
            else
            {
                VolSlider.Value = _prevvol;
                VolSlider.IsEnabled = true;
            }
        }

        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_loaded) return;
            Player.Instance.Volume = (float)VolSlider.Value;
        }

        private void PlayList_ItemDoubleClcik(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;

            ViewModel.PlayListIndex  = PlaylistView.SelectedIndex;
            if (ViewModel.PlayListIndex < 0) return;
            ViewModel.PlayListIndex = ViewModel.PlayListIndex + 1;

            StartPlay();
            SetPage(TabPage.NowPlaying);
        }

        private void _chapterprovider_ChapterClicked(object sender, double e)
        {
            _isdrag = true;
            Player.Instance.Position = e;
            _isdrag = false;
        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isdrag) return;
            if (SeekSlider.Maximum - SeekSlider.Value < 0.5)
            {
                if (CanDoNexPlaylisttTrack())
                {
                    NextPlaylistTrack();
                    StartPlay();
                }
                else
                {
                    Player.Instance.Stop();
                    ResetSongSeekUI();
                }
            }
        }

        private void SeekSlider_DragCompleted(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            Player.Instance.Position = SeekSlider.Value;
            _isdrag = false;
        }

        private void SeekSlider_DragStarted(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            _isdrag = true;
        }

        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LastVolume = (float)VolSlider.Value;
            Properties.Settings.Default.DeviceID = Player.Instance.CurrentDeviceID;
            App.SaveRecentUrls();
            Properties.Settings.Default.Save();
            if (App.NotifyIcon != null) App.NotifyIcon.RemoveIcon();
        }

        private void MainWin_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                DoLoadAndPlay(files);
            }
        }

        private void MainWin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (Properties.Settings.Default.ConfirmExit)
                {
                    var q = MessageBox.Show(Properties.Resources.MainWin_ExtitConfirmMessage, 
                                            Properties.Resources.MainWin_ExitConfirmTitle,
                                            MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (q == MessageBoxResult.Yes) Close();
                }
                else Close();
            }
        }

        private void DoOpen()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = string.Format("Auduio Files|{0}|Playlists|{1}", App.Formats, App.Playlists);
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DoLoadAndPlay(ofd.FileNames);   
            }
        }

        private void RadioStations_ItemDoubleClcik(object sender, RoutedEventArgs e)
        {
            DoLoadAndPlay(new string[] { RadioStations.SelectedUrl });
        }

        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            PlaylistLoaders.BuildAuidoCdMenu((MenuItem)sender, (items) =>
            {
                ViewModel.PlayList.AddRange(items);
                SetPage(TabPage.PlayList);
            });
        }

        public void SetPage(TabPage page)
        {
            int pageindex = (int)page;
            Dispatcher.Invoke(() => { MainView.SelectedIndex = pageindex; });
        }
    }
}
