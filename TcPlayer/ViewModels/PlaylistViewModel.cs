using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TcPlayer.Engine;

namespace TcPlayer.ViewModels;


internal partial class PlaylistViewModel : ObservableObject, IPlaylist
{
    [ObservableProperty]
    private int _currentIndex;

    public BindingList<EngineFile> Contents { get; }

    public PlaylistViewModel()
    {
        Contents = new BindingList<EngineFile>();
    }

    public EngineFile this[int index] => Contents[index];

    public int Count => Contents.Count;

    [RelayCommand]
    public void Shuffle() => Contents.Shuffle();

    [RelayCommand]
    public void Clear() => Contents.Clear();
}
