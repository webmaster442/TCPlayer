namespace TcPlayer.Services;

internal interface IDialogService
{
    void ErrorMessage(string title, string message);
    string? OpenFileDialog(OpenFileDialogSettings settings);
    string[]? OpenFilesDialog(OpenFileDialogSettings settings);
    string? SelectFolderDialog(string title);
}
