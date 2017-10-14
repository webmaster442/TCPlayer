using AppLib.Common.INI;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TCPlayer.Installer
{
    public interface IMainWindow: ICloseableView<MainWindowViewModel>
    {

    }

    public partial class MainWindowViewModel : ViewModel<IMainWindow>
    {
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand InstallListerCommand { get; private set; }
        public DelegateCommand InstallPackerCommand { get; private set; }
        public DelegateCommand InstallDesktopShortcutCommand { get; private set; }
        public DelegateCommand InstallStartMenuShortcutCommand { get; private set; }

        public MainWindowViewModel(IMainWindow mainWindow) : base(mainWindow)
        {
            ExitCommand = DelegateCommand.ToCommand(Exit);
            InstallListerCommand = DelegateCommand.ToCommand(InstallLister);
            InstallPackerCommand = DelegateCommand.ToCommand(InstallPacker);
            InstallDesktopShortcutCommand = DelegateCommand.ToCommand(InstallDesktopShortcut);
            InstallStartMenuShortcutCommand = DelegateCommand.ToCommand(InstallStartMenuShortcut);
        }

        /// <summary>
        /// Create a shortcut on desktop
        /// </summary>
        private void InstallDesktopShortcut()
        {
            CreateShortCut(Environment.SpecialFolder.Desktop);
        }

        /// <summary>
        /// Create a shortcut in start menu
        /// </summary>
        private void InstallStartMenuShortcut()
        {
            CreateShortCut(Environment.SpecialFolder.StartMenu);
        }

        /// <summary>
        /// Install packer plugin
        /// </summary>
        private void InstallPacker()
        {
            try
            {
                var FilesToCopy = new Dictionary<string, string>
                {
                    { "TCPlayerPacker.wcx", "\\plugins\\wcx\\tcplayerlister\\TCPlayerPacker.wcx" },
                    { "TCPlayerPacker.wcx64", "\\plugins\\wcx\\tcplayerlister\\TCPlayerPacker.wcx64" }
                };
                Install(FilesToCopy, (installfolder, ini) =>
                {
                    IniFile wincmd = IniFile.Open(ini);
                    wincmd.SetSetting("PackerPlugins", "tcplayer", "277," + Path.Combine(installfolder, "\\plugins\\wcx\\tcplayerlister\\TCPlayerPacker.wcx"));
                    wincmd.SaveToFile(ini);
                });
            }
            catch (Exception ex)
            {
                Error("Install Error:\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Install lister plugin
        /// </summary>
        private void InstallLister()
        {
            try
            {
                var FilesToCopy = new Dictionary<string, string>
                {
                    { "TCPlayerLister.wlx", "\\plugins\\wlx\\tcplayerlister\\TCPlayerLister.wlx" },
                    { "TCPlayerLister.wlx64", "\\plugins\\wlx\\tcplayerlister\\TCPlayerLister.wlx64" }
                };
                Install(FilesToCopy, (installfolder, ini) =>
                {
                    IniFile wincmd = IniFile.Open(ini);
                    int dumy;
                    var lastkey = (from setting in wincmd
                                   where setting.Key.Category == "ListerPlugins"
                                   && int.TryParse(setting.Key.SettingName, out dumy)
                                   orderby setting.Key.SettingName descending
                                   select setting.Key.SettingName).FirstOrDefault();

                    int keytoadd = Convert.ToInt32(lastkey) + 1;
                    wincmd.SetSetting("ListerPlugins", keytoadd.ToString(), Path.Combine(installfolder, "\\plugins\\wlx\\tcplayerlister\\TCPlayerLister.wlx"));
                    wincmd.SaveToFile(ini);
                });
            }
            catch (Exception ex)
            {
                Error("Install Error:\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Exit
        /// </summary>
        private void Exit()
        {
            View.Close();
        }
    }
}
