using AppLib.Common.Extensions;
using AppLib.Common.MessageHandler;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TCPlayer
{
    public partial class MainWinViewModel : ViewModel<IMainWindow>, IMessageClient<IEnumerable<string>>
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
