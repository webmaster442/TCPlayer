using System.Text;

namespace TcPlayer.Engine;

public static class FileExtensions
{
    private static readonly Dictionary<string, string> _fileExtensions = new()
    {
        { "Mp* files", "*.mp3;*.mp2;*.mp1" },
        { "Ogg files", "*.ogg" },
        { "Mp4 files", "*.m4a;*.m4b;*.mp4" },
        { "Aac files", "*.aac" },
        { "Ac3 files", "*.ac3" },
        { "Flac files", "*.flac" },
        { "Wma files", "*.wma" },
        { "Wavpack files", "*.wv" },
    };

    private static readonly Dictionary<string, string> _playlistExtensions = new()
    {
        { "M3u files", "*.m3u" },
        { "M3u8 files", "*.m3u8" },
        { "Pls files", "*.pls" },
        { "TcPlayer lists", "*.tcpls" }
    };

    public static string SupportedFilesFilter = CreateFilterString(_fileExtensions);

    public static string PlaylistFilesFilter = CreateFilterString(_playlistExtensions);

    private static string CreateFilterString(Dictionary<string, string> items)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Supported files|");
        sb.AppendJoin(';', items.Select(x => x.Value));
        foreach (var (key, value) in items)
        {
            sb.Append('|');
            sb.Append(key);
            sb.Append('|');
            sb.Append(value);
        }
        return sb.ToString();
    }

    public static IEnumerable<string> FilterSupportedItems(string[] files)
    {
        var extensions = _fileExtensions.SelectMany(x => x.Value.Split(';')).ToHashSet();
        foreach(var file in files)
        {
            if (extensions.Contains(Path.GetExtension(file)))
            {
                yield return file;
            }
        }
    }
}
