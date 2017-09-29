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
using System;
using System.Collections.Generic;
using System.Windows;
using TCPlayer.Code;
using TCPlayer.Lib;

namespace TCPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string AppName = "TCPlayer";
        internal const string Formats = "*.mp1;*.mp2;*.mp3;*.mp4;*.m4a;*.m4b;*.aac;*.flac;*.ac3;*.wv;*.wav;*.wma;*.asf;*.ogg;*.midi;*.mid;*.rmi;*.kar;*.xm;*.it;*.s3m;*.mod;*.mtm;*.umx;*.mo3;*.ape;*.mpc;*.mp+;*.mpp;*.ofr;*.ofs;*.spx;*.tta;*.dsf;*.dsdiff;*.opus";
        internal const string Playlists = "*.pls;*.m3u;*.wpl;*.asx";

        internal static Dictionary<string, string> CdData;
        internal static string DiscID;
        internal static NotificationIcon NotifyIcon;
        internal static HashSet<string> RecentUrls;

        [STAThread]
        public static void Main()
       {
            var si = new SingleInstanceApp(AppName);
            si.ReceiveString += Si_ReceiveString;
            if (si.IsFirstInstance)
            {
                var application = new App();
                CdData = new Dictionary<string, string>();
                DiscID = "";
                RecentUrls = new HashSet<string>();
                FillUrlList();
                application.InitializeComponent();
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
                application.Run();
                si.Close();
            }
            else si.SubmitParameters();
        }

        private static void FillUrlList()
        {
            if (!TCPlayer.Properties.Settings.Default.RememberRecentURLs) return;
            var items = TCPlayer.Properties.Settings.Default.RecentURLs.Split('\n', '\r');
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item)) continue;
                RecentUrls.Add(item);
            }
        }

        public static void SaveRecentUrls()
        {
            if (!TCPlayer.Properties.Settings.Default.RememberRecentURLs) return;
            var sb = new System.Text.StringBuilder();
            foreach (var url in RecentUrls) sb.AppendLine(url);
            TCPlayer.Properties.Settings.Default.RecentURLs = sb.ToString();
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
