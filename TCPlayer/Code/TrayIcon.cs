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
    public class TrayIcon
    {
        private NotifyIcon _icon;

        public TrayIcon()
        {
            _icon = new NotifyIcon
            {
                Visible = false,
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Text = Resources.Notify_Text,
            };
            _icon.DoubleClick += _icon_DoubleClick;
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

        public void MinimizeToTray()
        {
            _icon.Visible = true;
            App.Current.MainWindow.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void _icon_DoubleClick(object sender, EventArgs e)
        {
            _icon.Visible = false;
            App.Current.MainWindow.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
