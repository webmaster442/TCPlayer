using System.Configuration;
using System.Data;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using TcPlayer.Engine;
using TcPlayer.Services;
using TcPlayer.ViewModels;

namespace TcPlayer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider Services { get; }

    public App()
    {
        var playlist = new PlaylistViewModel();

        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IEngine, Engine.Engine>();
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<IPlaylist>(playlist);
        services.AddSingleton<PlaylistViewModel>(playlist);
        services.AddSingleton<IDialogService, DialogService>();

        Services = services.BuildServiceProvider();
    }
}
