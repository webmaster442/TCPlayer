using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace TCPlayer.Installer
{
    public partial class MainWindowViewModel : ViewModel<IMainWindow>
    {

        private void Notify(string s)
        {
            MessageBox.Show(s, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Error(string s)
        {
            MessageBox.Show(s, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Select total commander path
        /// </summary>
        /// <param name="tcpath">Total commander path</param>
        /// <returns>true, if totac commander selection was succesfull, false, if selection was canceled</returns>
        private bool SelectTCLocation(out string tcpath)
        {
            tcpath = "";
            var fb = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select Total Commander Path"
            };
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

        /// <summary>
        /// Run a command
        /// </summary>
        /// <param name="cmd"></param>
        private void RunCommand(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = cmd;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
        }

        /// <summary>
        /// Install
        /// </summary>
        /// <param name="CopyList">Files to copy. key: source location, value: target location</param>
        /// <param name="IniFileAction">Ini file action</param>
        private void Install(Dictionary<string, string> CopyList, Action<string, string> IniFileAction)
        {
            if (SelectTCLocation(out string installfolder))
            {
                int i = 0;
                foreach (var file in CopyList)
                {
                    var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file.Key);
                    var target = Path.Combine(installfolder, file.Value);
                    var cmd = $"/c copy {source} {target}";
                    RunCommand(cmd);
                    if (i == 0)
                    {
                        IniFileAction(installfolder, Path.Combine(installfolder, "wincmd.ini"));
                    }
                    i++;
                }
                CreateProgramLocFile(installfolder);
                RestartTC();
            }
        }

        /// <summary>
        /// Detect if TC is running. If running then offer to restart it.
        /// </summary>
        private void RestartTC()
        {
            Process[] commanders = Process.GetProcessesByName("totalcmd");
            if (commanders.Length > 0)
            {
                var q = MessageBox.Show("Total commander is running. Installed plugins will be available after restarting the program.\n" +
                                        "Do you want to restart it now?", "Total Commander running", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (q == MessageBoxResult.Yes)
                {
                    foreach (var commander in commanders)
                    {
                        var loc = commander.StartInfo.FileName;
                        commander.Kill();
                        Process.Start(loc);
                    }
                }
            }
        }

        /// <summary>
        /// Create Program loc file
        /// </summary>
        /// <param name="targetfolder"></param>
        private void CreateProgramLocFile(string targetfolder)
        {
            var file = Path.Combine(targetfolder, "program.loc");
            var content = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TCPlayer.exe");
            using (var loc = File.CreateText(file))
            {
                loc.WriteLine(content);
            }
        }

        /// <summary>
        /// Create Shortcut
        /// </summary>
        /// <param name="target"></param>
        private void CreateShortCut(Environment.SpecialFolder target)
        {
            IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(Environment.GetFolderPath(target) + "\\TCPlayer.lnk") as IWshRuntimeLibrary.IWshShortcut;
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
    }
}
