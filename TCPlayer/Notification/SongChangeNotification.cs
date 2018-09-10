using System.Windows;

namespace TCPlayer.Notification
{
    public class SongChangeNotification
    {
        private static NotificationWindow _window;

        public static void DisplaySongChangeNotification(NotificationPosition position, int time, string row1, string row2 = null)
        {
            if (_window != null && _window.Visibility == Visibility.Visible)
            {
                _window.ResetTimer(time);
                _window.Row1 = row1;
                _window.Row2 = row2;
                SetWindowPosition(position);
            }
            else
            {
                _window = new NotificationWindow(time);
                _window.Row1 = row1;
                _window.Row2 = row2;
                SetWindowPosition(position);
                _window.Show();
            }
        }

        private static void SetWindowPosition(NotificationPosition position)
        {
            double left = 0;
            double top = 0;
            double margin = 10;
            double screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            double screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            double windowWidth = _window.ActualWidth;
            double windowHeight = _window.ActualHeight;

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
