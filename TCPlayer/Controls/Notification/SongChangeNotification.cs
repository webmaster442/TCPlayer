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
using System.Windows;

namespace TCPlayer.Controls.Notification
{
    public class SongChangeNotification
    {
        private static NotificationWindow _window;
        private const int Time = 3000;

        public static void DisplaySongChangeNotification(string row1)
        {
            DisplaySongChangeNotification(row1, null, null);
        }

        public static void DisplaySongChangeNotification(string row1, string row2, string row3 = null)
        {
            var position = (NotificationPosition)Properties.Settings.Default.NotificationPlace;

            if (_window != null && _window.Visibility == Visibility.Visible)
            {
                _window.ResetTimer(Time);
                _window.Row1 = row1;
                _window.Row2 = row2;
                _window.Row3 = row3;
                SetWindowPosition(position);
                _window.Display();
            }
            else
            {
                _window = new NotificationWindow(Time);
                _window.Row1 = row1;
                _window.Row2 = row2;
                _window.Row3 = row3;
                SetWindowPosition(position);
                _window.Display();
            }
        }

        private static void SetWindowPosition(NotificationPosition position)
        {
            double left = 0;
            double top = 0;
            double margin = 10;
            double screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            double screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            double windowWidth = _window.Width;
            double windowHeight = _window.Height;

            switch (position)
            {
                case NotificationPosition.CenterBottom:
                case NotificationPosition.CenterMidle:
                case NotificationPosition.CenterTop:
                    left = (screenWidth - windowWidth) / 2;
                    break;
                case NotificationPosition.LeftBottom:
                case NotificationPosition.LeftMidle:
                case NotificationPosition.LeftTop:
                    left = margin;
                    break;
                case NotificationPosition.RightBottom:
                case NotificationPosition.RightMidle:
                case NotificationPosition.RightTop:
                    left = (screenWidth - windowWidth) - margin;
                    break;
            }

            switch (position)
            {
                case NotificationPosition.CenterTop:
                case NotificationPosition.LeftTop:
                case NotificationPosition.RightTop:
                    top = margin;
                    break;
                case NotificationPosition.CenterMidle:
                case NotificationPosition.LeftMidle:
                case NotificationPosition.RightMidle:
                    top = (screenHeight - windowHeight) / 2;
                    break;
                case NotificationPosition.CenterBottom:
                case NotificationPosition.LeftBottom:
                case NotificationPosition.RightBottom:
                    top = (screenHeight - windowHeight) - margin;
                    break;
            }

            _window.Left = left;
            _window.Top = top;
        }
    }
}
