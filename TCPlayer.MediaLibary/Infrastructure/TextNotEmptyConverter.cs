using AppLib.WPF.Converters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace TCPlayer.MediaLibary.Infrastructure
{
    public class TextNotEmptyConverter : ConverterBase<TextNotEmptyConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            return !string.IsNullOrEmpty(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
