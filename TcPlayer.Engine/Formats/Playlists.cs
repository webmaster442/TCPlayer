namespace TcPlayer.Engine.Formats;

public static class Playlists
{
    public static async Task<Result<IList<EngineFile>>> LoadM3U(TextReader reader, string filePath)
    {
        List<EngineFile> results = new();
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
            return new Result<IList<EngineFile>>(results);
        }
        catch (Exception e)
        {
            return new Result<IList<EngineFile>>(e);
        }
    }

    public static async Task<Result<IList<EngineFile>>> LoadPls(TextReader reader, string filePath)
    {
        string? line;
        int counter = 1;
        string basePath = Path.GetDirectoryName(filePath) ?? string.Empty;
        List<EngineFile> results = new();
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
            return new Result<IList<EngineFile>>(results);
        }
        catch (Exception e)
        {
            return new Result<IList<EngineFile>>(e);
        }
    }

    private static void AddWithAbsolutePath(List<EngineFile> results, string basePath, string candidate)
    {
        if (candidate.Contains('%'))
        {
            results.Add(EngineFile.FromFileName(candidate));
        }
        else
        {
            string fileName = Path.GetFullPath(candidate, basePath);
            results.Add(EngineFile.FromFileName(fileName));
        }
    }
}
