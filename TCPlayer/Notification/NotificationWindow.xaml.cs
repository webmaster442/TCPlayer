using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TCPlayer.Notification
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private DispatcherTimer _timer;
        private int _autoclose;
        private Storyboard _opening;
        private Storyboard _closing;


        public NotificationWindow(int autoclosetime = 2000)
        {
            InitializeComponent();
            _opening = FindResource("Opening") as Storyboard;
            _closing = FindResource("Closing") as Storyboard;
            _closing.Completed += _closing_Completed;
            _timer = new DispatcherTimer();
            _autoclose = autoclosetime;
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += _timer_Tick;
            _timer.Start();
            Loaded += NotificationWindow_Loaded;
        }

        private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BeginStoryboard(_opening);
        }

        private void _closing_Completed(object sender, EventArgs e)
        {
            this.Close();
            _closing.Completed -= _closing_Completed;
        }

        public string Row1
        {
            get { return TbRow1.Text; }
            set { TbRow1.Text = value; }
        }

        public string Row2
        {
            get { return TbRow2.Text; }
            set { TbRow2.Text = value; }
        }

        public void ResetTimer(int autoclosetime = 2000)
        {
            _timer.Stop();
            _autoclose = autoclosetime;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_autoclose > 0)
            {
                _autoclose -= (int)_timer.Interval.TotalMilliseconds;
            }
            else
            {
                _timer.Stop();
                BeginStoryboard(_closing);
            }
        }
    }
}
