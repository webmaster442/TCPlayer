// --------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This software is licensed under the MIT license. See LICENSE file for details.
// --------------------------------------------------------------------------------------------
using System;
using System.Windows.Input;

namespace TCPluginInstaller
{
    internal class ActionCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public ActionCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
