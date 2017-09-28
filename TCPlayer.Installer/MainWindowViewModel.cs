using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLib.WPF.MVVM;

namespace TCPlayer.Installer
{
    public interface IMainWindow: IView
    {

    }

    public class MainWindowViewModel: ViewModel<IMainWindow>
    {
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand InstallListerCommand { get; private set; }
        public DelegateCommand InstallPackerCommand { get; private set; }
        public DelegateCommand InstallShortcutCommand { get; private set; }

        public MainWindowViewModel()
        {
            ExitCommand = DelegateCommand.ToCommand(Exit);
            InstallListerCommand = DelegateCommand.ToCommand(InstallLister);
            InstallPackerCommand = DelegateCommand.ToCommand(InstallPacker);
            InstallShortcutCommand = DelegateCommand.ToCommand(InstallShortcut);
        }

        private void InstallShortcut()
        {
            throw new NotImplementedException();
        }

        private void InstallPacker()
        {
            throw new NotImplementedException();
        }

        private void InstallLister()
        {
            throw new NotImplementedException();
        }

        private void Exit()
        {
            View.Close();
        }
    }
}
