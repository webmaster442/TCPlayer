using System;
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
            string fullpath = value.ToString();

            if (fullpath.StartsWith("http") || fullpath.StartsWith("cd:"))
            {
                return fullpath;
            }
            else
            {
                try
                {
                    TagLib.File tags = TagLib.File.Create(fullpath);
                    var artist = tags.Tag.Performers[0];
                    var title = tags.Tag.Title;
                    if (string.IsNullOrEmpty(artist)) artist = "Unknown artist";
                    if (string.IsNullOrEmpty(title)) title = "Unknown song";
                    return string.Format("{0} - {1}", artist, title);

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
