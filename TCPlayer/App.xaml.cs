using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TCPlayer.Code;

namespace TCPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string AppName = "TCPlayer";
        internal const string Formats = "*.mp3;*.mp4;*.m4a;*.m4b;*.aac;*.flac;*.ac3;*.wv;*.wav;*.wma;*.ogg";
        internal const string Playlists = "*.pls;*.m3u;*.wpl";

        internal static Dictionary<string, string> _cddata;
        internal static string _discid;

        [STAThread]
        public static void Main()
        {
            var si = new SingleInstanceApp(AppName);
            si.ReceiveString += Si_ReceiveString;
            if (si.IsFirstInstance)
            {
                var application = new App();
                _cddata = new Dictionary<string, string>();
                _discid = "";
                application.InitializeComponent();
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
                application.Run();
                si.Close();
            }
            else si.SubmitParameters();
        }

        private static void Si_ReceiveString(string obj)
        {
            var files = obj.Split('\n');
            App.Current.Dispatcher.Invoke(() =>
            {
                var mw = App.Current.MainWindow as MainWindow;
                mw.DoLoadAndPlay(files);
            });
        }
    }
}
