using CommunityToolkit.Mvvm.ComponentModel;

using TcPlayer.Engine.Formats;

namespace TcPlayer.ViewModels;

internal partial class AddUrlViewModel : ObservableObject
{
    [ObservableProperty]
    private string _url;

    public AddUrlViewModel()
    {
        _url = string.Empty;
    }

    public bool IsValid
    {
        get
        {
            return !string.IsNullOrEmpty(Url)
                && (Url.StartsWith("http://") || Url.StartsWith("https://"));
        }
    }

    public PlaylistItem GetPlaylistItem()
        => new(Url, Engine.EngineFileType.Network, Url);
}
