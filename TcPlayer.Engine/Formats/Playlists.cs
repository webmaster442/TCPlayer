namespace TcPlayer.Engine.Formats;

public static class Playlists
{
    private const string Http = "http://";
    private const string Https = "https://";

    public static async Task<Result<IList<PlaylistItem>>> LoadM3U(TextReader reader, string filePath)
    {
        List<PlaylistItem> results = new();
        string basePath = Path.GetDirectoryName(filePath) ?? string.Empty;
        try
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith('#'))
                {
                    continue;
                }
                AddWithAbsolutePath(results, basePath, line);
            }
            return new Result<IList<PlaylistItem>>(results);
        }
        catch (Exception e)
        {
            return new Result<IList<PlaylistItem>>(e);
        }
    }

    public static async Task<Result<IList<PlaylistItem>>> LoadPls(TextReader reader, string filePath)
    {
        string? line;
        int counter = 1;
        string basePath = Path.GetDirectoryName(filePath) ?? string.Empty;
        List<PlaylistItem> results = new();
        try
        {
            while ((line = await reader.ReadLineAsync()) != null)
            {
                string search = $"File{counter}=";
                if (line.StartsWith(search))
                {
                    string candidate = line.Substring(search.Length);
                    AddWithAbsolutePath(results, basePath, candidate);
                    ++counter;
                }
            }
            return new Result<IList<PlaylistItem>>(results);
        }
        catch (Exception e)
        {
            return new Result<IList<PlaylistItem>>(e);
        }
    }

    public static async Task WriteM3U(TextWriter writer, IList<PlaylistItem> items, string basePath)
    {
        foreach (var item in items)
        {
            var line = GetRelativePath(basePath, item.Path);
            await writer.WriteLineAsync(line);
        }
    }

    public static PlaylistItem FromFile(string basePath, string candidate)
    {
        if (candidate.Contains('%'))
        {
            if (candidate.Contains('%'))
            {
                var expanded = Environment.ExpandEnvironmentVariables(candidate);
                return new PlaylistItem(expanded, EngineFileType.File, GetMetaData(expanded));
            }
            else
            {
               return new PlaylistItem(candidate, EngineFileType.File, GetMetaData(candidate));
            }
        }
        else
        {
            string fileName = Path.GetFullPath(candidate, basePath);
            return new PlaylistItem(fileName, EngineFileType.File, GetMetaData(fileName));
        }
    }

    private static void AddWithAbsolutePath(List<PlaylistItem> results, string basePath, string candidate)
    {
        if (candidate.StartsWith(Http)
            || candidate.StartsWith(Https))
        {
            results.Add(new PlaylistItem(candidate, EngineFileType.Network, candidate));
        }
        else
        {
            results.Add(FromFile(basePath, candidate));
        }
    }

    private static string GetMetaData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return filePath;
        }

        using var file = TagLib.File.Create(filePath);

        if (string.IsNullOrWhiteSpace(file.Tag.FirstPerformer)
            && string.IsNullOrWhiteSpace(file.Tag.Title))
        {
            return filePath;
        }
        return $"{file.Tag.FirstPerformer} - {file.Tag.Title}";

    }

    public static string GetRelativePath(string basePath, string targetPath)
    {
        static string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        Uri baseUri = new(AppendDirectorySeparatorChar(basePath));
        Uri targetUri = new(targetPath);

        Uri relativeUri = baseUri.MakeRelativeUri(targetUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        return relativePath.Replace('/', Path.DirectorySeparatorChar);
    }
}
