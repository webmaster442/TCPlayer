using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLib.WPF.MVVM;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using System.Diagnostics;
using AppLib.Common.INI;

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

        private void Notify(string s)
        {
            MessageBox.Show(s, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Error(string s)
        {
            MessageBox.Show(s, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool SelectTCLocation(out string tcpath)
        {
            tcpath = "";
            var fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.Description = "Select Total Commander Path";
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var exe = Path.Combine(fb.SelectedPath, "TOTALCMD.EXE");
                var exe64 = Path.Combine(fb.SelectedPath, "TOTALCMD64.EXE");
                var ini = Path.Combine(fb.SelectedPath, "wincmd.ini");

                if (File.Exists(ini) &&
                    (File.Exists(exe) || File.Exists(exe64)))
                {
                    tcpath = fb.SelectedPath;
                    return true;
                }
                else
                {
                    Error("No Total Commander Instalation was found at the specified folder");
                    return false;
                }

            }
            return false;
        }

        private void RunCommand(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = cmd;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
        }

        private void Install(Dictionary<string, string> CopyList)
        {
            string installfolder;
            if (SelectTCLocation(out installfolder))
            {
                int index = 0;
                foreach(var file in CopyList)
                {
                    var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file.Key);
                    var target = Path.Combine(installfolder, file.Value);
                    var cmd = $"/c copy {source} {target}";
                    RunCommand(cmd);
                    if (index == 0)
                    {
                        IniFile wincmd = IniFile.Open(Path.Combine(installfolder, "wincmd.ini"));
                        //wincmd.SetSetting("PackerPlugins", "tcplayer", "277," + target);
                    }
                    index++;
                }
            }
        }

        private void InstallShortcut()
        {
            IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TCPlayer.lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.Arguments = "";
            shortcut.TargetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TCPlayer.exe");
            shortcut.WindowStyle = 1;
            shortcut.Description = "Total Commander Player";
            shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            shortcut.IconLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TCPlayer.exe");
            shortcut.Save();
            Marshal.ReleaseComObject(shortcut);
            Marshal.ReleaseComObject(wsh);
            Notify("Shortcut Created");
        }

        private void InstallPacker()
        {
            var FilesToCopy = new Dictionary<string, string>();
            FilesToCopy.Add("TCPlayerPacker.wcx", "\\plugins\\wcx\\tcplayerlister\\TCPlayerPacker.wcx");
            FilesToCopy.Add("TCPlayerPacker.wcx64", "\\plugins\\wcx\\tcplayerlister\\TCPlayerPacker.wcx64");
            Install(FilesToCopy);
        }

        private void InstallLister()
        {
            var FilesToCopy = new Dictionary<string, string>();
            FilesToCopy.Add("TCPlayerLister.wlx", "\\plugins\\wlx\\tcplayerlister\\TCPlayerLister.wlx");
            FilesToCopy.Add("TCPlayerLister.wlx64", "\\plugins\\wlx\\tcplayerlister\\TCPlayerLister.wlx64");
            Install(FilesToCopy);
        }

        private void Exit()
        {
            View.Close();
        }
    }
}
