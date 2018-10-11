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
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TCPlayer.Converters
{
    /// <summary>
    /// Converts a full path name to a file name. Internally calls System.IO.Path.GetFileName
    /// </summary>
    /// <seealso cref="System.IO.Path.GetFileName"/>
    [ValueConversion(typeof(string), typeof(string))]
    public class FileNameConverter : MarkupExtension, IValueConverter
    {
        private Dictionary<string, string> _cache;
        private const int _limit = 300;

        public FileNameConverter()
        {
            _cache = new Dictionary<string, string>(_limit);
        }

        /// <summary>
        /// Converts a full path to a file name
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>string, filename</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fullpath = value as string;

            if (string.IsNullOrEmpty(fullpath)) return Binding.DoNothing;

            if (fullpath.StartsWith("http") || fullpath.StartsWith("cd:"))
            {
                return fullpath;
            }
            else
            {
                try
                {
                    if (_cache.ContainsKey(fullpath))
                    {
                        return _cache[fullpath];
                    }
                    else
                    {
                        TagLib.File tags = TagLib.File.Create(fullpath);
                        var artist = tags.Tag.Performers[0];
                        var title = tags.Tag.Title;
                        if (string.IsNullOrEmpty(artist)) artist = "Unknown artist";
                        if (string.IsNullOrEmpty(title)) title = "Unknown song";
                        
                        if (_cache.Count + 1 > _limit)
                        {
                            _cache.Clear();
                        }
                        _cache.Add(fullpath, $"{artist} - {title}");
                        return _cache[fullpath];
                    }

                }
                catch (Exception)
                {
                    var fname = System.IO.Path.GetFileName(fullpath);
                    return fname;
                }
            }
        }

        /// <summary>
        /// Returns the unmodified input
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>unmodified inpt</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
