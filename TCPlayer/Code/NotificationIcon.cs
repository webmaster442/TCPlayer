using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TCPlayer.Code
{
    internal class NotificationIcon: IDisposable
    {
        private NotifyIcon _icon;

        public NotificationIcon()
        {
            _icon = new NotifyIcon();
            _icon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            _icon.Text = "TC Player is running";
            _icon.Visible = true;
        }

        protected virtual void Dispose(bool native)
        {
            if (_icon != null)
            {
                _icon.Dispose();
                _icon = null;
            }
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void ShowTrackChange(string artist, string title)
        {
            var text = string.Format("Artist: {0}\r\nTitle: {1}", artist, title);
            _icon.BalloonTipText = text;
            _icon.BalloonTipTitle = "Now playing";
            _icon.ShowBalloonTip(500);
        }
    }
}
