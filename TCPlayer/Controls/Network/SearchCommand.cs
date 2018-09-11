using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace TCPlayer.Controls.Network
{
    public class SearchCommand : ICommand
    {
        public string UrlParameter { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var provider = parameter as NetworkSearchProvider;

            return (provider != null && !string.IsNullOrEmpty(provider.UriTemplate));
        }

        public void Execute(object parameter)
        {
            var provider = parameter as NetworkSearchProvider;
            if (provider != null)
            {
                try
                {
                    var str = provider.GetFullUri(UrlParameter);
                    Process.Start(str);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Properties.Resources.Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
