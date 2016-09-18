﻿/*
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

namespace TCPlayer.Code
{
    internal static class Helpers
    {
        public static void ErrorDialog(Exception ex, string description = null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (description != null)
                {
                    MessageBox.Show(string.Format("{0}\r\nDetails:{1}", description, ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

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
