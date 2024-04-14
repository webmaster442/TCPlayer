using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TcPlayer.ViewModels;

internal partial class PlayerControlsViewModel : ObservableObject
{
    [ObservableProperty]
    public double _currentPosition;

    [ObservableProperty]
    public double _totalTime;

    [RelayCommand]
    public void PlayPause()
    {

    }
}
