using System.Windows;

namespace TcPlayer.Services;

internal class DialogService : IDialogService
{
    public void ErrorMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public string[] OpenFiles()
    {
        throw new NotImplementedException();
    }
}
