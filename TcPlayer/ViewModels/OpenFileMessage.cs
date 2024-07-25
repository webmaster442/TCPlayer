namespace TcPlayer.ViewModels;

internal class OpenFileMessage
{
    public string[] Files { get; }

    public OpenFileMessage(string[] files)
    {
        Files = files;
    }
}
