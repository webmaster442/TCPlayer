/*
TC Plyer
Total Commander Audio Player plugin & standalone player written in C#, based on bass.dll components
Copyright (C) 2016 Webmaster442 aka. Ruzsinszki Gábor

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
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

        private void Notify([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowModel()
        {
            InstallCommand = new ActionCommand(Install, CanInstall);
            BrowseCommand = new ActionCommand(Browse, CanDoOtherStuff);
            ExitCommand = new ActionCommand(Exit, CanDoOtherStuff);
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
            openFileDialog.Filter = "Ini Files | *.ini";
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
    }
}
