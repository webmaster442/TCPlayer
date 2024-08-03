using Microsoft.Extensions.DependencyInjection;

using TcPlayer.Engine;
using TcPlayer.Services;
using TcPlayer.ViewModels;

namespace TcPlayer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : SingleInstanceApp
{
    public IServiceProvider Services { get; }

    public App()
    {
        var dialogs = new DialogService();
        var playlist = new PlaylistViewModel(dialogs);

        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IDialogService>(dialogs);
        services.AddSingleton<IEngine, Engine.Engine>();
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<IPlaylist>(playlist);
        services.AddSingleton<PlayerControlsViewModel>();
        services.AddSingleton<PlaylistViewModel>(playlist);

        Services = services.BuildServiceProvider();
    }

    public override void HandleArguments(string[] args)
    {
        if (args.Length > 0)
        {
            Services.GetRequiredService<IMediator>()
                .Notify(new OpenFileMessage(args));
        }
    }
}
