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
        Commands =
        [
            new MenuCommand
            {
                Name = "File",
                Childs =
                [
                    new MenuCommand()
                    {
                        Name = "Load list...",
                        Command = LoadListCommand,
                    },
                    new MenuCommand()
                    {
                        Name = "Append list...",
                        Command = AppendListCommand,
                    },
                ]
            },
            new MenuCommand
            {
                Name = "Add",
                Childs =
                [
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
                ]
            },
            new MenuCommand
            {
                Name = "Remove",
                Childs =
                [
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
                ],
            },
            new MenuCommand
            {
                Name = "Sort",
                Childs =
                [
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
                ]
            }
        ];
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
            Filter = FileExtensions.SupportedFilesFilter,
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

    public static async Task<Result<IList<PlaylistItem>>> Load(string file)
    {
        if (string.IsNullOrEmpty(file)
            || !File.Exists(file))
        {
            return new Result<IList<PlaylistItem>>(new FileNotFoundException($"{file} not found"));
        }

        switch (Path.GetExtension(file).ToLower())
        {
            case ".m3u":
            case ".m3u8":
                {
                    using var rader = File.OpenText(file);
                    return await Playlists.LoadM3U(rader, file);
                }
            case ".pls":
                {
                    using var reader = File.OpenText(file);
                    return await Playlists.LoadPLS(reader, file);
                }
            case ".tcpls":
                {
                    using var stream = File.OpenRead(file);
                    return await Playlists.LoadJson(stream, file);
                }
            default:
                return new Result<IList<PlaylistItem>>(new NotSupportedException($"Unsupported playlist format: {file}"));
        }
    }

    [RelayCommand]
    public void AddUrl()
    {
        var model = new AddUrlViewModel();
        if (_dialogService.CustomDialog(model, "Add url..."))
        {
            if (!model.IsValid)
            {
                _dialogService.ErrorMessage("Error", "Not a valid url");
                return;
            }
            Contents.Add(model.GetPlaylistItem());
        }
    }


    [RelayCommand]
    public async Task AppendList()
    {
        var selectedFile = _dialogService.OpenFileDialog(new OpenFileDialogSettings
        { 
            Filter = FileExtensions.PlaylistFilesFilter,
            Title = "Append playlist..." 
        });

        if (selectedFile == null)
            return;

        _dialogService.BusyIndicator(true, "Appending playlist...");
        var result = await Load(selectedFile);
        _dialogService.BusyIndicator(false, string.Empty);

        result.Handle(items => Contents.AddRange(items),
        ex => _dialogService.ErrorMessage("Error", ex.Message));
    }

    [RelayCommand]
    public async Task LoadList()
    {
        Contents.Clear();
        await AppendList();
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
