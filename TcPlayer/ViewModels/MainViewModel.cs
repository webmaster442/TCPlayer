using CommunityToolkit.Mvvm.ComponentModel;

using TcPlayer.Engine;
using TcPlayer.Services;

namespace TcPlayer.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    public PlayerControlsViewModel Player { get; }
    public PlaylistViewModel PlaylistViewModel { get; }

    public MainViewModel(IEngine engine,
                         IDialogService dialogService,
                         IMediator mediator,
                         PlaylistViewModel playlistViewModel)
    {
        Player = new PlayerControlsViewModel(engine, dialogService, mediator);
        PlaylistViewModel = playlistViewModel;
    }
}
