using System.ComponentModel;

namespace TcPlayer.Services;

internal interface IDialogService
{
    void ErrorMessage(string title, string message);
    string? OpenFileDialog(OpenFileDialogSettings settings);
    string[]? OpenFilesDialog(OpenFileDialogSettings settings);
    string? SelectFolderDialog(string title);
    void BusyIndicator(bool isBusy, string message);
    bool CustomDialog(INotifyPropertyChanged content, string title);
}
