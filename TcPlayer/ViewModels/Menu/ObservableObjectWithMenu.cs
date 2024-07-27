using CommunityToolkit.Mvvm.ComponentModel;

namespace TcPlayer.ViewModels.Menu;

internal abstract class ObservableObjectWithMenu : ObservableObject
{
    public MenuCommand[] Commands { get; protected set; }

    public ObservableObjectWithMenu()
    {
        Commands = Array.Empty<MenuCommand>();
    }

    private static void Refresh(MenuCommand[] children)
    {
        foreach (var child in children)
        {
            child.Command?.NotifyCanExecuteChanged();
            Refresh(child.Childs);
        }
    }

    public void RefreshCommands()
        => Refresh(Commands);
}