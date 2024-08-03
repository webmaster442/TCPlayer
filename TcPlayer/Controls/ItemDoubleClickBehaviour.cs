using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TcPlayer.Controls;
internal class ItemDoubleClickBehaviour
{
    public static readonly DependencyProperty ItemDoubleClickCommandProperty = DependencyProperty.RegisterAttached(
        "ItemDoubleClickCommand",
        typeof(ICommand),
        typeof(ItemDoubleClickBehaviour),
        new PropertyMetadata(null, OnItemDoubleClickCommandChanged));

    public static ICommand GetItemDoubleClickCommand(DependencyObject obj)
    {
        return (ICommand)obj.GetValue(ItemDoubleClickCommandProperty);
    }

    public static void SetItemDoubleClickCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(ItemDoubleClickCommandProperty, value);
    }

    private static void OnItemDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Selector selector)
        {
            if (e.NewValue != null)
            {
                selector.MouseDoubleClick += OnMouseDoubleClick;
            }
            else
            {
                selector.MouseDoubleClick -= OnMouseDoubleClick;
            }
        }
    }

    private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is Selector selector 
            && selector.SelectedItem != null)
        {
            ICommand command = GetItemDoubleClickCommand(selector);
            if (command?.CanExecute(selector.SelectedItem) == true)
            {
                command.Execute(selector.SelectedItem);
            }
        }
    }
}
