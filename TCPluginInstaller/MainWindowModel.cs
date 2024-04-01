// --------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This software is licensed under the MIT license. See LICENSE file for details.
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;

namespace TCPluginInstaller
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
        private string _iniPath;
        private bool _installLister;
        private bool _installPacker;
        private bool _installing;

        public event PropertyChangedEventHandler PropertyChanged;

        public ActionCommand InstallCommand { get; }
        public ActionCommand ExitCommand { get; }
        public ActionCommand BrowseCommand { get; }
        public ActionCommand RegistryLocateCommand { get; }
        public ActionCommand DefaultLocateCommand { get; }

        private void Notify([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowModel()
        {
            InstallCommand = new ActionCommand(Install, CanInstall);
            BrowseCommand = new ActionCommand(Browse, CanDoOtherStuff);
            ExitCommand = new ActionCommand(Exit, CanDoOtherStuff);
            RegistryLocateCommand = new ActionCommand(LocateViaRegistry);
            DefaultLocateCommand = new ActionCommand(LocateDefault);

        }

        public bool IsInstalling
        {
            get { return _installing; }
            set
            {
                if (value != _installing)
                {
                    _installing = value;
                    InstallCommand.RaiseCanExecuteChanged();
                    BrowseCommand.RaiseCanExecuteChanged();
                    ExitCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string IniPath
        {
            get { return _iniPath; }
            set
            {
                if (value != _iniPath)
                {
                    _iniPath = value;
                    Notify();
                    InstallCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool InstallLister
        {
            get { return _installLister; }
            set
            {
                if (value != _installLister)
                {
                    _installLister = value;
                    Notify();
                    InstallCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool InstallPacker
        {
            get { return _installPacker; }
            set
            {
                if (value != _installPacker)
                {
                    _installPacker = value;
                    Notify();
                    InstallCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private bool CanDoOtherStuff(object arg)
        {
            return !IsInstalling;
        }

        private void Browse(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Total commander configuration|wincmd.ini";
            if (openFileDialog.ShowDialog() == true)
            {
                IniPath = openFileDialog.FileName;
            }
        }

        public bool CanInstall(object parameter)
        {
            return (InstallLister || InstallPacker) &&
                    (!string.IsNullOrEmpty(IniPath) &&
                    System.IO.File.Exists(IniPath) && !IsInstalling);
        }

        public void Install(object parameter)
        {
            IsInstalling = true;
            try
            {
                Logic.TCPluginInstaller.Install(IniPath, InstallLister, InstallPacker);
                MessageBox.Show("Installation completed", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                App.Current.MainWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsInstalling = false;
            }
        }

        public void Exit(object parameter)
        {
            App.Current.MainWindow.Close();
        }

        public void LocateViaRegistry(object param)
        {
            object found = null;
            var IniKey = Registry.CurrentUser.OpenSubKey(@"Software\Ghisler\Total Commander\");
            found = IniKey?.GetValue("IniFileName");

            if (found!= null)
            {
                IniPath = Convert.ToString(found);
            }
            else
            {
                IniKey = Registry.LocalMachine.OpenSubKey(@"Software\Ghisler\Total Commander\");
                found = IniKey?.GetValue("IniFileName");
                if (found != null)
                {
                    IniPath = Convert.ToString(found);
                }
                else
                {
                    MessageBox.Show("Ini Coudn't be found. Probably bad installation?", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public void LocateDefault(object param)
        {
            IniPath = Environment.ExpandEnvironmentVariables(@"%AppData%\Ghisler\wincmd.ini");
        }
    }
}
