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

        public void ShowNotification(string filename, string artist = null, string title = null)
        {
            _icon.BalloonTipTitle = "Now playing";
            string text = null;
            if (string.IsNullOrEmpty(artist) && string.IsNullOrEmpty(title))
            {
                text = filename;
            }
            else
            {
                if (string.IsNullOrEmpty(artist)) artist = "Unknown";
                if (string.IsNullOrEmpty(title)) title = "Unknown song";
                text = string.Format("{0}\r\nArtist: {1}\r\nTitle: {2}", filename, artist, title);
            }
            _icon.BalloonTipText = text;
            _icon.ShowBalloonTip(500);
        }

        public void RemoveIcon()
        {
            _icon.Visible = false;
        }
    }
}
