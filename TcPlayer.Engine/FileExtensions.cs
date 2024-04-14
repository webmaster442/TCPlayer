using System.Text;

namespace TcPlayer.Engine;

public sealed class FileExtensions
{
    private readonly Dictionary<string, string> _fileExtensions = new()
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

    public string CreateFilterString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Supported files|");
        sb.Append(string.Join(';', _fileExtensions.Select(x => x.Value)));
        return sb.ToString();
    }

}
