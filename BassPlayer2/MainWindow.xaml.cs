using TCPlayer.Code;
using TCPlayer.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;

namespace TCPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private IntPtr hwnd;
        private HwndSource hsource;
        private Player _player;
        private float _prevvol;
        private DispatcherTimer _timer;
        private bool _loaded;
        private bool _isdrag;

        public MainWindow()
        {
            InitializeComponent();
            _player = new Player();
            _player.ChangeDevice(); //init
            _prevvol = 1.0f;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.IsEnabled = false;
            _timer.Tick += _timer_Tick;
            _loaded = true;
        }

        private void MainWin_SourceInitialized(object sender, EventArgs e)
        {
            if ((hwnd = new WindowInteropHelper(this).Handle) == IntPtr.Zero)
            {
                throw new InvalidOperationException("Could not get window handle.");
            }

            hsource = HwndSource.FromHwnd(hwnd);
            hsource.AddHook(WndProc);
            TitleBar.Background = new SolidColorBrush(GetWindowColorizationColor(false));
        }

        private static Color GetWindowColorizationColor(bool opaque)
        {
            var par = new DWMCOLORIZATIONPARAMS();
            Native.DwmGetColorizationParameters(ref par);

            return Color.FromArgb(
                (byte)(opaque ? 255 : par.ColorizationColor >> 24),
                (byte)(par.ColorizationColor >> 16),
                (byte)(par.ColorizationColor >> 8),
                (byte)par.ColorizationColor);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                //WM_DWMCOLORIZATIONCOLORCHANGED
                case 0x320:
                    TitleBar.Background = new SolidColorBrush(GetWindowColorizationColor(false));
                    return IntPtr.Zero;
                default:
                    return IntPtr.Zero;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void TitlebarClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitlebarMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutDialog();
            ShowDialog(about);
        }

        public static void ShowDialog(UserControl dialog)
        {
            var main = Application.Current.MainWindow as MainWindow;
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
            dialog.OkClicked?.Invoke();
            OverLayContent.Children.Clear();
        }

        protected virtual void Dispose(bool native)
        {
            if (_player != null)
            {
                _player.Dispose();
                _player = null;
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void DoLoadAndPlay(IEnumerable<string> items)
        {
            PlayList.DoLoad(items);
            PlayList.NextTrack();
            StartPlay();
        }

        private void BtnChangeDev_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            var selector = new DeviceChange();
            string[] devices = _player.GetDevices();
            selector.DataContext = devices;
            selector.OkClicked= new Action(() =>
            {
                var name = devices[selector.DeviceIndex];
                _player.ChangeDevice(name);
            });
            MainWindow.ShowDialog(selector);
        }

        private void Reset()
        {
            _timer.IsEnabled = false;
            SeekSlider.Value = 0;
            Taskbar.ProgressState = TaskbarItemProgressState.Normal;
            Taskbar.ProgressValue = 0;
            TbCurrTime.Text = TimeSpan.FromSeconds(0).ToShortTime();
            TbFullTime.Text = TimeSpan.FromSeconds(0).ToShortTime();
        }


        private void StartPlay()
        {
            try
            {
                if (!_loaded || PlayList.SelectedItem == null) return;
                Reset();
                SongDat.Reset();
                _player.Load(PlayList.SelectedItem);

                if (_player.IsStream)
                {
                    Taskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
                }
                else
                {
                    var len = TimeSpan.FromSeconds(_player.Length);
                    TbFullTime.Text = len.ToShortTime();
                    SeekSlider.Maximum = _player.Length;
                }
                _player.Play();
                _timer.IsEnabled = true;
                SongDat.UpdateMediaInfo(PlayList.SelectedItem);

            }
            catch (Exception ex)
            {
                _timer.IsEnabled = false;
                Reset();
                SongDat.Reset();
                Helpers.ErrorDialog(ex);
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (!_loaded) return;
            if (_isdrag)
            {
                TbCurrTime.Text = TimeSpan.FromSeconds(SeekSlider.Value).ToShortTime();
                return;
            }
            var pos = TimeSpan.FromSeconds(_player.Position);
            TbCurrTime.Text = pos.ToShortTime();
            if (_player.IsStream)
            {
                SeekSlider.Value = 0;
            }
            else
            {
                SeekSlider.Value = _player.Position;
                Taskbar.ProgressValue = SeekSlider.Value / SeekSlider.Maximum;
            }

        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isdrag) return;
            if (SeekSlider.Maximum - SeekSlider.Value < 0.5)
            {
                PlayList.NextTrack();
                StartPlay();
            }
        }

        private void DoAction(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            var btn = sender as Button;
            if (btn == null) return;
            switch (btn.Name)
            {
                case "BtnPlayPause":
                    _player.PlayPause();
                    break;
                case "BtnStop":
                    _player.Stop();
                    break;
                case "BtnSeekBack":
                    _timer.IsEnabled = false;
                    _player.Position -= 5;
                    _timer.IsEnabled = true;
                    break;
                case "BtnSeekFwd":
                    _timer.IsEnabled = false;
                    _player.Position += 5;
                    _timer.IsEnabled = true;
                    break;
                case "BtnNextTrack":
                    PlayList.NextTrack();
                    StartPlay();
                    break;
                case "BtnPrevTrack":
                    PlayList.PreviousTrack();
                    StartPlay();
                    break;
            }
            _timer.IsEnabled = !_player.IsPaused;
            if (_player.IsPaused) Taskbar.ProgressState = TaskbarItemProgressState.Paused;
            else if (!_player.IsStream) Taskbar.ProgressState = TaskbarItemProgressState.Normal;
            else Taskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
        }

        private void ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            var btn = sender as ThumbButtonInfo;
            if (btn == null) return;
            switch (btn.Description)
            {
                case "Play/Pause":
                    _player.PlayPause();
                    break;
                case "Previous track":
                    PlayList.PreviousTrack();
                    StartPlay();
                    break;
                case "Next track":
                    PlayList.NextTrack();
                    StartPlay();
                    break;
                case "Mute/UnMute":
                    var state = (bool)BtnMute.IsChecked;
                    BtnMute.IsChecked = !state;
                    BtnMute_Click(null, null);
                    break;
            }
            _timer.IsEnabled = !_player.IsPaused;
            if (_player.IsPaused) Taskbar.ProgressState = TaskbarItemProgressState.Paused;
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
            _player.Volume = (float)VolSlider.Value;
        }

        private void PlayList_ItemDoubleClcik(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            StartPlay();
            Dispatcher.Invoke(() =>
            {
                MainView.SelectedIndex = 0;
            });
        }

        private void SeekSlider_DragCompleted(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            _player.Position = SeekSlider.Value;
            _isdrag = false;
        }

        private void SeekSlider_DragStarted(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            _isdrag = true;
        }

        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
