using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TCPlayer.Style
{
    /// <summary>
    /// Converts a full path name to a file name. Internally calls System.IO.Path.GetFileName
    /// </summary>
    /// <seealso cref="System.IO.Path.GetFileName"/>
    [ValueConversion(typeof(string), typeof(string))]
    public class FileNameConverter : IValueConverter
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
    }
}
