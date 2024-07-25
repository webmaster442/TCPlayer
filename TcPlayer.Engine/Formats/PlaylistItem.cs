namespace TcPlayer.Engine.Formats;

public class PlaylistItem
{
    public string Metadata { get; init; }
    public string Path { get; init; }
    public EngineFileType FileType { get; init; }

    public PlaylistItem(string path, EngineFileType fileType, string metadata)
    {
        Metadata = metadata;
        Path = path;
        FileType = fileType;
    }

    public override string ToString()
        => Path;
}
