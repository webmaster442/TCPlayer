using System.ComponentModel;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TcPlayer.Engine;
using TcPlayer.Engine.Formats;
using TcPlayer.Services;
using TcPlayer.ViewModels.Menu;

namespace TcPlayer.ViewModels;


internal partial class PlaylistViewModel : ObservableObject, IPlaylist, IMenuCommands
{
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private int _currentIndex;

    [ObservableProperty]
    public int _selectedIndex;

    partial void OnSelectedIndexChanged(int value)
    {
        if (value != CurrentIndex)
            CurrentIndex = value;
    }

    public BindingList<PlaylistItem> Contents { get; }

    public PlaylistViewModel(IDialogService dialogService)
    {
        Contents = new BindingList<PlaylistItem>();
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

    public EngineFile this[int index] => EngineFile.FromPlaylistItem(Contents[index]);

    public int Count => Contents.Count;

    public MenuCommand[] Commands { get; }

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
                Contents.Add(Playlists.FromFile(file, file));
            }
        }
    }

    [RelayCommand]
    public void AddFolder()
    {
        var result = _dialogService.SelectFolderDialog("Add folder...");
        if (result != null)
        {
            var files = Directory.GetFiles(result);
            foreach (var file in FileExtensions.FilterSupportedItems(files))
            {
                Contents.Add(Playlists.FromFile(file, file));
            }
        }
    }

    [RelayCommand]
    public void AddUrl()
    {

    }

    [RelayCommand]
    public void RemoveSelected()
    {
        int wasSelected = SelectedIndex;
        Contents.RemoveAt(SelectedIndex);
        SelectedIndex = wasSelected;
    }

    [RelayCommand]
    public void RemoveAll() => Contents.Clear();

    [RelayCommand]
    public void RemoveAllExceptSelected()
    {
        var item = Contents[SelectedIndex];
        Contents.Clear();
        Contents.Add(item);
    }

    [RelayCommand]
    public void SortByAz()
    {
        var sorted = Contents.OrderBy(x => x.Path);
        Contents.Clear();
        Contents.AddRange(sorted);
    }

    [RelayCommand]
    public void SortByZa()
    {
        var sorted = Contents.OrderByDescending(x => x.Path);
        Contents.Clear();
        Contents.AddRange(sorted);
    }

    [RelayCommand]
    public void Shuffle() => Contents.Shuffle();
}
