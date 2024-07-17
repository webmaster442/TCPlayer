using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using TcPlayer.Engine;

namespace TcPlayer.Converters;

internal sealed class SelectedDeviceNameConverter : MarkupExtension, IValueConverter
{
    // VM -> UI
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DeviceInfo deviceInfo)
        {
            return $"{deviceInfo.Name} ({deviceInfo.Channels})";
        }
        return Binding.DoNothing;
    }

    // UI -> VM
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
