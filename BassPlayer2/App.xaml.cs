using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BassPlayer2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "BassPlayer2";

        public const string Formats = "*.mp3;*.mp4;*.m4a;*.m4b;*.aac;*.flac;*.ac3;*.wv;*.wav;*.wma;*.ogg";

        public const string Playlists = "*.pls;*.m3u;*.wpl";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
                application.Run();
                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            var main = Current.MainWindow as MainWindow;
            main.DoLoadAndPlay(args);
            return true;
        }
    }
}
