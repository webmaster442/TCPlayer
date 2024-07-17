using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

using TcPlayer.ViewModels.Menu;

namespace TcPlayer.Controls;

internal class TabItemToMenuCommands : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TabItem tabItem)
        {
            if (tabItem.Content is Control control
                && control.DataContext is IMenuCommands menuCommands
                && menuCommands.Commands.Length > 0)
            {
                return menuCommands.Commands;
            }
            else
            {
                return Array.Empty<MenuCommand>();
            }
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
