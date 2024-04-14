using CommunityToolkit.Mvvm.ComponentModel;

namespace TcPlayer.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    public PlayerControlsViewModel Player { get; }

    public MainViewModel()
    {
        Player = new PlayerControlsViewModel();
    }
}
