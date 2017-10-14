using System.Windows;
using AppLib.WPF.MVVM;

namespace TCPlayer.Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SetViewModel(new MainWindowViewModel(this));
        }

    }
}
