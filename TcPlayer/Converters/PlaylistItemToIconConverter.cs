using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using TcPlayer.Engine.Formats;

namespace TcPlayer.Converters;
internal class PlaylistItemToIconConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PlaylistItem item)
        {
            if (!File.Exists(item.Path))
            {
                return CreatePath("M20 17H22V15H20V17M20 7V13H22V7M4 2C2.89 2 2 2.89 2 4V20C2 21.11 2.89 22 4 22H16C17.11 22 18 21.11 18 20V8L12 2M4 4H11V9H16V20H4Z", Colors.Black);
            }

            return item.FileType switch
            {
                Engine.EngineFileType.Network => CreatePath("M15,20A1,1 0 0,0 14,19H13V17H17A2,2 0 0,0 19,15V5A2,2 0 0,0 17,3H7A2,2 0 0,0 5,5V15A2,2 0 0,0 7,17H11V19H10A1,1 0 0,0 9,20H2V22H9A1,1 0 0,0 10,23H14A1,1 0 0,0 15,22H22V20H15M7,15V5H17V15H7Z", Colors.Black),
                Engine.EngineFileType.File => CreatePath("M14,2L20,8V20A2,2 0 0,1 18,22H6A2,2 0 0,1 4,20V4A2,2 0 0,1 6,2H14M18,20V9H13V4H6V20H18M13,10V12H11V17A2,2 0 0,1 9,19A2,2 0 0,1 7,17A2,2 0 0,1 9,15C9.4,15 9.7,15.1 10,15.3V10H13Z", Colors.Black),
                Engine.EngineFileType.Cd => CreatePath("M12,14C10.89,14 10,13.1 10,12C10,10.89 10.89,10 12,10C13.11,10 14,10.89 14,12A2,2 0 0,1 12,14M12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20A8,8 0 0,0 20,12A8,8 0 0,0 12,4Z", Colors.Black),
                _ => throw new UnreachableException(),
            };
        }
        return Binding.DoNothing;
    }

    private static System.Windows.Shapes.Path CreatePath(string pathData, Color color)
    {
        return new System.Windows.Shapes.Path
        {
            Data = Geometry.Parse(pathData),
            Fill = new SolidColorBrush(color),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
