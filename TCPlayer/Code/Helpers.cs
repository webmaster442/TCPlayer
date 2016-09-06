using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace TCPlayer.Code
{
    internal static class Helpers
    {
        public static void ErrorDialog(Exception ex, string description = null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (description != null)
                {
                    MessageBox.Show(string.Format("{0}\r\nDetails:{1}", description, ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }
}
