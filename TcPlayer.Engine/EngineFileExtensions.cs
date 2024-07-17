namespace TcPlayer.Engine;

public static class EngineFileExtensions
{
    public static bool TryGetFileName(this EngineFile file, out string fileName)
    {
        if (file.FileType != EngineFileType.File
            || string.IsNullOrEmpty(file.Uri)
            || !file.Uri.StartsWith("file://"))
        {
            fileName = string.Empty;
            return false;
        }

        fileName = file.Uri.Replace("file://", string.Empty);
        return true;
    }
}
