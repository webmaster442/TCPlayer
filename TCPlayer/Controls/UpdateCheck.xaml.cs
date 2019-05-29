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
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using TCPlayer.Code;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for UpdateCheck.xaml
    /// </summary>
    public partial class UpdateCheck : UserControl, IDialog
    {
        public class UpdateResult
        {
            public bool Succes { get; set; }
            public bool Error { get; set; }
            public Version Latest { get; set; }
        }

        private const string UpdatePathUrl = "https://raw.githubusercontent.com/webmaster442/TCPlayer/master/LatestVersion.txt";
        private const int UpdateCheckDays = 7;

        public Action OkClicked { get; set; }

        public UpdateCheck(Version v)
        {
            InitializeComponent();
            Header.Text = string.Format(Properties.Resources.UpdateCheck_Header, v);
        }

        public static async Task<UpdateResult> CheckForUpdate()
        {

            if (!Properties.Settings.Default.CheckForUpdates)
                return CreateFail();

            var lastCheck = DateTime.UtcNow - Properties.Settings.Default.LastUpdateCheck;

            if (lastCheck < TimeSpan.FromDays(UpdateCheckDays))
                return CreateFail();

            try
            {
                using (WebClient client = new WebClient())
                {
                    string versionString = await client.DownloadStringTaskAsync(UpdatePathUrl);
                    if (Version.TryParse(versionString, out Version parsed))
                    {
                        if (parsed > GetCurrentVersion())
                        {
                            Properties.Settings.Default.LastUpdateCheck = DateTime.UtcNow;
                            Properties.Settings.Default.Save();
                            return new UpdateResult
                            {
                                Succes = true,
                                Latest = parsed,
                                Error = false
                            };
                        }
                    }
                }
                return CreateFail();
            }
            catch (WebException)
            {
                return CreateFail(true);
            }
        }

        private static UpdateResult CreateFail(bool error = false)
        {
            return new UpdateResult
            {
                Succes = false,
                Error = error,
                Latest = new Version()
            };
        }

        private static Version GetCurrentVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
