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
using System.Windows.Threading;
using TCPlayer.Properties;

namespace TCPlayer.Code
{
    internal static class Helpers
    {
        /// <summary>
        /// Creates an error dialog
        /// </summary>
        /// <param name="ex">Exception message</param>
        /// <param name="description">Average human readable error</param>
        public static void ErrorDialog(Exception ex, string description = null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (description != null)
                {
                    MessageBox.Show($"{description}\n{Resources.Error_Details}\n{ex.Message}", Resources.Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else MessageBox.Show(ex.Message, Resources.Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        /// <summary>
        /// Returns true, if the parameter file is a midi
        /// </summary>
        /// <param name="file">file to check</param>
        /// <returns>true, if midi, false if not</returns>
        public static bool IsMidi(string file)
        {
            var ext = System.IO.Path.GetExtension(file);
            switch (ext)
            {
                case ".midi":
                case ".mid":
                case ".rmi":
                case ".kar":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true, if the parameter file is a tracker format
        /// </summary>
        /// <param name="file">file to check</param>
        /// <returns>true, if tracker, false if not</returns>
        public static bool IsTracker(string file)
        {
            var ext = System.IO.Path.GetExtension(file);
            switch (ext)
            {
                case ".xm":
                case ".it":
                case ".s3m":
                case ".mod":
                case ".mtm":
                case ".umx":
                case ".mo3":
                    return true;
                default:
                    return false;
            }
        }
    }
}
