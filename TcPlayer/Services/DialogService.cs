using System.ComponentModel;
using System.Windows;

using Microsoft.Win32;

using TcPlayer.Controls;

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

    public bool CustomDialog(INotifyPropertyChanged content, string title)
    {
        var dialog = new DialogWindow
        {
            Owner = Application.Current.MainWindow,
            Title = title,
            MaxWidth = Application.Current.MainWindow.Width * 0.8,
            MaxHeight = Application.Current.MainWindow.Height * 0.8,
        };
        dialog.MainContent.Content = content;
        return dialog.ShowDialog() == true;
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
