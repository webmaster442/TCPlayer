using System.Windows;

namespace TcPlayer.Controls;
/// <summary>
/// Interaction logic for DialogWindow.xaml
/// </summary>
public partial class DialogWindow : Window
{
    public DialogWindow()
    {
        InitializeComponent();
    }

    private void OkClick(object sender, RoutedEventArgs e)
        => DialogResult = true;

    private void CancelClick(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
