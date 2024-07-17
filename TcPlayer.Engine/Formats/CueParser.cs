namespace TcPlayer.Engine.Formats.Cue;

public static class CueParser
{
    private static List<string[]> CreateChunks(string[] lines)
    {
        List<string[]> chunks = new();
        int start = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("TRACK ") || i == lines.Length - 1)
            {
                if (start != -1)
                {
                    string[] chunk = new string[i - start];
                    Array.Copy(lines, start, chunk, 0, i - start);
                    chunks.Add(chunk);
                }
                start = i;
            }
        }

        return chunks;
    }

    private static string Get(string[] chunk, string key, bool quotes)
    {
        foreach (var line in chunk)
        {
            string s = line.Trim();
            if (s.StartsWith(key))
            {
                if (quotes)
                {
                    int end = s.Length - key.Length - 2;
                    return s.Substring(key.Length + 1, end);
                }
                else
                {
                    int end = s.Length - key.Length;
                    return s.Substring(key.Length, end);
                }
            }
        }
        throw new InvalidOperationException(key + " not found");
    }

    public static IEnumerable<Chapter> Parse(string[] lines)
    {
        bool rekordBox = lines.Any(line => line == "REM RECORDED_BY \"rekordbox-dj\"");
        foreach (var chunk in CreateChunks(lines))
        {
            var time = Get(chunk, "INDEX 01 ", quotes: false);

            yield return new Chapter()
            {
                Title = $"{Get(chunk, "PERFORMER ", quotes: true)} - {Get(chunk, "TITLE ", quotes: true)}",
                StartTimeSeconds = TimeSpan.Parse(time).TotalSeconds,
            };
        }
    }
} 
