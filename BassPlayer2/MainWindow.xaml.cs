using BassPlayer2.Code;
using BassPlayer2.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BassPlayer2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private Player _player;

        public MainWindow()
        {
            InitializeComponent();
            _player = new Player();
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

        private void DoAction(object sender, RoutedEventArgs e)
        {
            var tbtn = sender as ToggleButton;
            if (tbtn == null)
            {
                var btn = sender as Button;
                if (btn == null) return;
                switch (btn.Name)
                {

                }
            }
            else
            {
                switch (tbtn.Name)
                {

                }
            }
        }
    }
}
