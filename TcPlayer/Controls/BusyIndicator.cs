using System.Windows;

namespace TcPlayer.Controls;

internal class BusyIndicator : ExtendedControl
{
    public string Message
    {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message", typeof(string), typeof(BusyIndicator), new PropertyMetadata(string.Empty));
}
