using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using TcPlayer.ViewModels.Menu;

namespace TcPlayer.Controls;
internal class TabMenuControl : Menu
{
    public TabMenuControl()
    {
        RenderTransform = new ScaleTransform(1.4, 1.4);
    }

    public MenuCommand[] MenuCommands
    {
        get { return (MenuCommand[])GetValue(MenuCommandsProperty); }
        set { SetValue(MenuCommandsProperty, value); }
    }

    public static readonly DependencyProperty MenuCommandsProperty =
        DependencyProperty.Register("MenuCommands", typeof(MenuCommand[]), typeof(TabMenuControl), new PropertyMetadata(null, MenuChange));

    private static void MenuChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TabMenuControl control)
        {
            control.Items.Clear();
            if (control.MenuCommands != null)
            {
                Fill(control.Items, control.MenuCommands);
            }
        }
    }

    private static void Fill(ItemCollection items, MenuCommand[] childs)
    {
        foreach (var command in childs)
        {
            var item = new MenuItem
            {
                Header = command.Name,
                Command = command.Command,
            };
            Fill(item.Items, command.Childs);
            items.Add(item);
        }
    }
}
