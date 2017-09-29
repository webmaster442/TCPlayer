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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TCPlayer.Properties;

namespace TCPlayer.Code
{
    internal class NotificationIcon: IDisposable
    {
        private NotifyIcon _icon;

        public NotificationIcon()
        {
            _icon = new NotifyIcon();
            _icon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            _icon.Text = Resources.Notify_Text;
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
            _icon.BalloonTipTitle = Resources.Notify_Title;
            string text = null;
            if (string.IsNullOrEmpty(artist) && string.IsNullOrEmpty(title))
            {
                text = filename;
            }
            else
            {
                if (string.IsNullOrEmpty(artist)) artist = Resources.SongData_UnknownArtist;
                if (string.IsNullOrEmpty(title)) title = Resources.SongData_UnknownSong;
                text = string.Format("{0}\r\n{1} - {2}", filename, artist, title);
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
