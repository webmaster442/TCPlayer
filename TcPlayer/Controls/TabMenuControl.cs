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
        foreach (var child in childs)
        {
            if (child == MenuCommand.Seperator)
            {
                items.Add(new Separator());
                continue;
            }

            var item = new MenuItem
            {
                Header = child.Name,
                Command = child.Command,
            };
            Fill(item.Items, child.Childs);
            items.Add(item);
        }
    }
}
