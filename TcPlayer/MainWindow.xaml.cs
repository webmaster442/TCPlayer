using System.ComponentModel;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using TcPlayer.ViewModels;

namespace TcPlayer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static T Resolve<T>() where T : notnull
    {
        return ((App)App.Current).Services.GetRequiredService<T>();
    }

    private bool InDesignMode()
    {
        return DesignerProperties.GetIsInDesignMode(this);
    }

    public MainWindow()
    {
        InitializeComponent();
        if (!InDesignMode())
        {
            DataContext = new MainViewModel(
                Resolve<PlayerControlsViewModel>(),
                Resolve<PlaylistViewModel>());
        }
    }
}