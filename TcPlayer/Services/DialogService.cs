using System.Windows;

namespace TcPlayer.Services;

internal class DialogService : IDialogService
{
    public void ErrorMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public string? OpenFileDialog(OpenFileDialogSettings settings)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = settings.Title,
            Filter = settings.Filter,
            Multiselect = false,
            CheckFileExists = true,
            ShowReadOnly = true,
            CheckPathExists = true,
        };
        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }
        return null;
    }

    public string[]? OpenFilesDialog(OpenFileDialogSettings settings)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = settings.Title,
            Filter = settings.Filter,
            Multiselect = true,
            CheckFileExists = true,
            ShowReadOnly = true,
            CheckPathExists = true,
        };
        if (dialog.ShowDialog() == true)
        {
            return dialog.FileNames;
        }
        return null;
    }
}
