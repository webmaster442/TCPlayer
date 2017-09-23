using AppLib.Common.MessageHandler;
using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using TCPlayer.MediaLibary.DB;
using TCPlayer.Properties;

namespace TCPlayer.MediaLibary
{
    public partial class MediaLibaryViewModel : ViewModel<IMediaLibary>
    {
        private async void MenuAddFiles()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Audio Files|" + App.Formats;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await DataBase.Instance.AddFiles(ofd.FileNames);
            }
        }

        private async void MenuAddFolder()
        {
            string[] filters = App.Formats.Split(';');
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = Resources.Playlist_AddFolderDescription;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> Files = new List<string>(30);
                foreach (var filter in filters)
                {
                    Files.AddRange(Directory.GetFiles(fbd.SelectedPath, filter));
                }
                Files.Sort();
                await DataBase.Instance.AddFiles(Files);
            }
        }

        private void MenuSendToPlaylist()
        {
            Messager.Instance.SendMessage(SelectedItems);
        }

        private void MenuCreateQuery()
        {
            throw new NotImplementedException();
        }

        private void MenuBackupDb()
        {
            throw new NotImplementedException();
        }

    }
}
