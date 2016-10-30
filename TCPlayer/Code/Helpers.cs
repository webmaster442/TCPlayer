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
using System.Text;
using System.Windows;
using System.Windows.Threading;

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
                    MessageBox.Show(string.Format("{0}\r\nDetails:{1}", description, ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        /// <summary>
        /// Serializes an eq preset to a setting
        /// </summary>
        /// <param name="presets">eq parameters</param>
        public static void SaveEqPresets(float[] presets)
        {
            var sb = new StringBuilder();
            for (int i=0; i<presets.Length; i++)
            {
                var isend = i == (presets.Length - 1);
                sb.Append(presets[i]);
                if (!isend) sb.Append(";");
            }
            Properties.Settings.Default.EqualizerPreset = sb.ToString();
        }

        /// <summary>
        /// Deserializes an eq setting from app settings
        /// </summary>
        /// <returns>an eq preset</returns>
        public static float[] LoadEqPresets()
        {
            float[] ret = new float[10];
            if (string.IsNullOrEmpty(Properties.Settings.Default.EqualizerPreset))
            {
                return ret;
            }
            else
            {
                try
                {
                    var parts = Properties.Settings.Default.EqualizerPreset.Split(';');
                    if (parts.Length != 10) throw new Exception("Partially saved preset");

                    var temp = 0.0f;
                    for (int i=0; i<10; i++)
                    {
                        var succes = float.TryParse(parts[i], out temp);
                        if (succes) ret[i] = temp;
                        else ret[i] = 0;
                    }

                    return ret;
                }
                catch (Exception ex)
                {
                    Helpers.ErrorDialog(ex, "Equalizer preset load failed");
                    for (int i = 0; i < 10; i++) ret[i] = 0;
                    return ret;
                }
            }
        }
    }
}
