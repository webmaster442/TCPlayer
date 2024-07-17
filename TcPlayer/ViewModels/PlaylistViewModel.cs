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
                Name = "Add",
                Childs = new[]
                {
                    new MenuCommand
                    {
                        Name = "Add files...",
                        Command = AddFilesCommand,
                    },
                    new MenuCommand
                    {
                        Name = "Add folder...",
                        Command = AddFolderCommand,
                    },
                    new MenuCommand
                    {
                        Name = "Add URL...",
                        Command = AddUrlCommand,
                    },
                }
            },
            new MenuCommand
            {
                Name = "Remove",
                Childs = new[]
                {
                    new MenuCommand
                    {
                        Name = "Remove selected",
                        Command = RemoveSelectedCommand,
                    },
                    new MenuCommand
                    {
                        Name = "Remove all",
                        Command = RemoveAllCommand,
                    },
                    new MenuCommand
                    {
                        Name = "Remove all except selected",
                        Command = RemoveAllExceptSelectedCommand,
                    },
                },
            },
            new MenuCommand
            {
                Name = "Sort",
                Childs = new[]
                {
                    new MenuCommand
                    {
                        Name = "Sort A -> Z",
                        Command = SortByAzCommand,
                    },
                    new MenuCommand
                    {
                        Name = "Sort Z -> A",
                        Command = SortByZaCommand,
                    },
                    new MenuCommand
                    {
                        Name = "Shuffle",
                        Command = ShuffleCommand,
                    },
                }
            }
        };
    }

    public EngineFile this[int index] => Contents[index];

    public int Count => Contents.Count;

    public MenuCommand[] Commands
    {
        get;
    }

    [RelayCommand]
    public void AddFiles()
    {
        var result = _dialogService.OpenFilesDialog(new OpenFileDialogSettings
        {
            Title = "Add files...",
            Filter = FileExtensions.CreateFilterString(),
        });
        if (result != null)
        {
            foreach (var file in result)
            {
                Contents.Add(EngineFile.FromFileName(file));
            }
        }
    }

    [RelayCommand]
    public void AddFolder()
    {

    }

    [RelayCommand]
    public void AddUrl()
    {

    }

    [RelayCommand]
    public void RemoveSelected()
    {
    }

    [RelayCommand]
    public void RemoveAll() => Contents.Clear();

    [RelayCommand]
    public void RemoveAllExceptSelected()
    {

    }

    [RelayCommand]
    public void SortByAz()
    {

    }

    [RelayCommand]
    public void SortByZa()
    {

    }

    [RelayCommand]
    public void Shuffle() => Contents.Shuffle();
}
