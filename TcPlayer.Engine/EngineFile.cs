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
        if (fileName.Contains('%'))
        {
            var expanded = Environment.ExpandEnvironmentVariables(fileName);
            return new($"file://{expanded}", EngineFileType.File);
        }

        return new($"file://{fileName}", EngineFileType.File);
    }

    public static EngineFile FromUrl(string url)
    {
       return new(url, EngineFileType.Network);
    }
}
