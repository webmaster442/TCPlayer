using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TCPlayer.Code;

namespace TCPlayer.Style
{
    public class PositionToTimeStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is double)
            {
                double input = (double)value;
                TimeSpan ts = TimeSpan.FromSeconds(input);
                return ts.ToShortTime();
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
