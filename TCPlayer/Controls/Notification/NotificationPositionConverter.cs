using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TCPlayer.Controls.Notification
{
    public class NotificationPositionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                var i = (int)value;
                return (NotificationPosition)i;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is NotificationPosition)
            {
                var i = (NotificationPosition)value;
                return (int)i;
            }
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
