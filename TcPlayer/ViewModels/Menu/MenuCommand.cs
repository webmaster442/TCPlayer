using CommunityToolkit.Mvvm.Input;

namespace TcPlayer.ViewModels.Menu;
internal class MenuCommand
{
    public required string Name { get; init; }
    public IRelayCommand? Command { get; init; }
    public MenuCommand[] Childs { get; init; }

    public static MenuCommand Seperator = new() { Name = "-" };

    public MenuCommand()
    {
        Childs = Array.Empty<MenuCommand>();
    }
}
