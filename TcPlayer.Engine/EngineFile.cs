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

    public static EngineFile FromFileName(string fileName)
    {
        return new($"file://{fileName}", EngineFileType.File);
    }
}
