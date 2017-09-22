using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLib.WPF.MVVM;
using System.Windows.Controls;

namespace TCPlayer
{
    public enum TabPage
    {
        NowPlaying = 0,
        PlayList = 1,
        FileBrowser = 2,
        Radio = 3
    }

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
        /// <summary>
        /// Set Main window page
        /// </summary>
        /// <param name="page">Page id</param>
        void SetPage(TabPage page);
        /// <summary>
        /// Selected files in the file explorer
        /// </summary>
        IEnumerable<string> FileExplorerSelectedFiles { get; }
        /// <summary>
        /// Load items to playlist and play them
        /// </summary>
        /// <param name="items">Items to add</param>
        void DoLoadAndPlay(IEnumerable<string> items);
        /// <summary>
        /// Play a single file
        /// </summary>
        /// <param name="item">file to play</param>
        void DoLoadAndPlay(string item);
    }
}
