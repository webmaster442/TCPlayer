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

            this.ViewModelAction<MainWindowViewModel>(vm =>
            {
                vm.View = this;
            });
        }
    }
}
