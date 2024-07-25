using TcPlayer.Engine.Formats;

namespace TcPlayer.Engine;

public sealed class EngineFile
{
    private EngineFile(string uri, EngineFileType fileType)
    {
        Uri = uri;
        FileType = fileType;
    }

    public string Uri { get; }
    public EngineFileType FileType { get; }

    public static EngineFile FromPlaylistItem(PlaylistItem item)
    {
        if (item.FileType == EngineFileType.File)
        {
            return FromFileName(item.Path);
        }
        return new(item.Path, item.FileType);
    }

    public static EngineFile FromFileName(string fileName)
    {
        return new($"file://{fileName}", EngineFileType.File);
    }

    public static EngineFile FromUrl(string url)
    {
        return new(url, EngineFileType.Network);
    }
}
