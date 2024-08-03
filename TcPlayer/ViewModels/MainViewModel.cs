using CommunityToolkit.Mvvm.ComponentModel;

using TcPlayer.Engine;
using TcPlayer.Services;

namespace TcPlayer.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    public PlayerControlsViewModel Player { get; }
    public PlaylistViewModel PlaylistViewModel { get; }

    public MainViewModel(PlayerControlsViewModel playerControlsViewModel, 
                         PlaylistViewModel playlistViewModel)
    {
        Player = playerControlsViewModel;
        PlaylistViewModel = playlistViewModel;
    }
}
