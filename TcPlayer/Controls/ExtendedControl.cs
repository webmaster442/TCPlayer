using System.Windows;
using System.Windows.Controls;

namespace TcPlayer.Controls;

/// <summary>
/// Control extended with usefull methods
/// </summary>
internal abstract class ExtendedControl: Control
{
    protected T GetTemplateChild<T>(string name) where T : DependencyObject
    {
        return GetTemplateChild(name) is T casted
            ? casted
            : throw new InvalidOperationException($"{name} not found or type mismatch");
    }
}
