using BassPlayer2.Code;
using BassPlayer2.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace BassPlayer2
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
                (byte) par.ColorizationColor);
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
            (OverLayContent.Children[0] as IDialog).OkClicked();
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

        private void StartPlay()
        {
            if (!_loaded) return;
            _player.Load(PlayList.SelectedItem);
            _player.Play();
            _timer.IsEnabled = true;
            var len = TimeSpan.FromSeconds(_player.Length);
            TbFullTime.Text = len.ToShortTime();
            SeekSlider.Value = 0;
            SongDat.UpdateMediaInfo(PlayList.SelectedItem);
            SeekSlider.Maximum = _player.Length;
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
            SeekSlider.Value = _player.Position;
            if (SeekSlider.Value == SeekSlider.Maximum)
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
                    _isdrag = true;
                    SeekSlider.Value -= 5;
                    _isdrag = false;
                    break;
                case "BtnSeekFwd":
                    _isdrag = true;
                    SeekSlider.Value += 5;
                    _isdrag = false;
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
    }
}
