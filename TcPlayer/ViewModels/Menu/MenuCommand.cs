using System.Windows.Input;

namespace TcPlayer.ViewModels.Menu;
internal class MenuCommand
{
    public required string Name { get; init; }
    public ICommand? Command { get; init; }
    public MenuCommand[] Childs { get; init; }

    public MenuCommand()
    {
       Childs = Array.Empty<MenuCommand>();
    }
}
