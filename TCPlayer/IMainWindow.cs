using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLib.WPF.MVVM;
using System.Windows.Controls;

namespace TCPlayer
{
    public interface IMainWindow: IView
    {
        /// <summary>
        /// Show a user control in the popup
        /// </summary>
        /// <param name="dialog">Dialog to show</param>
        void ShowDialog(UserControl dialog);
        /// <summary>
        /// Delete selected items from the playlist
        /// </summary>
        void DeleteSelectedFromPlaylist();
    }
}
