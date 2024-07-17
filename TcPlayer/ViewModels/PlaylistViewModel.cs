using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TcPlayer.Engine;
using TcPlayer.Services;
using TcPlayer.ViewModels.Menu;

namespace TcPlayer.ViewModels;


internal partial class PlaylistViewModel : ObservableObject, IPlaylist, IMenuCommands
{
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private int _currentIndex;

    public BindingList<EngineFile> Contents { get; }

    public PlaylistViewModel(IDialogService dialogService)
    {
        Contents = new BindingList<EngineFile>();
        _dialogService = dialogService;
        Commands = new MenuCommand[]
        {
            new MenuCommand
            {
                Name = "Shuffle",
                Command = ShuffleCommand,
            },
            new MenuCommand
                        {
                Name = "Clear",
                Command = ClearCommand,
            },
        };
    }

    public EngineFile this[int index] => Contents[index];

    public int Count => Contents.Count;

    public MenuCommand[] Commands
    {
        get;
    }

    [RelayCommand]
    public void Shuffle() => Contents.Shuffle();

    [RelayCommand]
    public void Clear() => Contents.Clear();
}
