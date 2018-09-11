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
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TCPlayer.Controls.Notification
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
        }

        internal void Display()
        {
            Show();
            BeginStoryboard(_opening);
        }

        private void _closing_Completed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
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

        public string Row3
        {
            get { return TbRow3.Text; }
            set { TbRow3.Text = value; }
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
