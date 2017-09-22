using AppLib.Common.Extensions;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPlayer
{
    public partial class MainWinViewModel : ViewModel<IMainWindow>
    {
        private void FileExplorerPlaySelected()
        {
            View.SetPage(TabPage.PlayList);
            var selected = View.FileExplorerSelectedFiles.FirstOrDefault();
            View.DoLoadAndPlay(selected);
        }

        private void FileExplorerAddToPlaylist()
        {
            PlayList.AddRange(View.FileExplorerSelectedFiles);
            View.SetPage(TabPage.PlayList);
        }

        private void FileExplorerNavigateHome()
        {
            throw new NotImplementedException();
        }
    }
}
