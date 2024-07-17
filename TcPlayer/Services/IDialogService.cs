namespace TcPlayer.Services;

internal interface IDialogService
{
    string[] OpenFiles();
    void ErrorMessage(string title, string message);
}
