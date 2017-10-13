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
using AppLib.WPF.MVVM;
using System.Collections.Generic;
using System.Windows.Controls;
using TCPlayer.Code;

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
        /// <summary>
        /// Render the file explorer home view
        /// </summary>
        void FileExplorerHomeView();
    }
}
