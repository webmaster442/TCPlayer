using System.Windows;

using Microsoft.Win32;

namespace TcPlayer.Services;

internal sealed class DialogService : IDialogService
{
    public void BusyIndicator(bool isBusy, string message)
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.BusyIndicator.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
            mainWindow.BusyIndicator.Message = message;
        }
    }

    public void ErrorMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public string? OpenFileDialog(OpenFileDialogSettings settings)
    {
        var dialog = new OpenFileDialog
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
        var dialog = new OpenFileDialog
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

    public string? SelectFolderDialog(string title)
    {
        var dialog = new OpenFolderDialog
        {
            Title = title,
            Multiselect = false
        };
        if (dialog.ShowDialog() == true)
        {
            return dialog.FolderName;
        }
        return null;
    }
}
