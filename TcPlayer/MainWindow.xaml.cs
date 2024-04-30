using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using TcPlayer.Engine;
using TcPlayer.Services;
using TcPlayer.ViewModels;

namespace TcPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static T Resolve<T>() where T : notnull
        {
            return ((App)App.Current).Services.GetRequiredService<T>();
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(
                Resolve<IEngine>(),
                Resolve<IDialogService>(),
                Resolve<IMediator>());
        }
    }
}