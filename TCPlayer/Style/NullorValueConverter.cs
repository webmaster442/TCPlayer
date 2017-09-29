using System;
using System.Globalization;
using System.Windows.Data;

namespace TCPlayer.Style
{
    /// <summary>
    /// Displays a value if the value is not null. IF null, then the parameter is returned
    /// </summary>
    public class NullorValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to value if the value is not null. IF null, then the parameter is returned
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>value if the value is not null. IF null, then the parameter is returned</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return parameter;
            var str = value as string;

            if (string.IsNullOrEmpty(str)) return parameter;
       
            return value;
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
