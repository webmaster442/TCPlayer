using System.Windows;
using System.Windows.Controls;

namespace TcPlayer.Controls
{
    internal class ExtendedControl: Control
    {
        protected T GetTemplateChild<T>(string name) where T : DependencyObject
        {
            if (GetTemplateChild(name) is T casted)
            {
                return casted;
            }
            throw new InvalidOperationException($"{name} not found or type mismatch");
        }
    }
}
