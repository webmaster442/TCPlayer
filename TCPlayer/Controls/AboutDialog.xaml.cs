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
using TCPlayer.Code;
using System;
using System.Windows.Controls;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.IO;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : UserControl, IDialog
    {
        private void BtnWebsite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://webmaster442.github.io/TCPlayer/");
        }

        private void LoadAbout(string fname = null)
        {
            try
            {
                var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                Uri f = null;
                if (string.IsNullOrEmpty(fname)) f = new Uri("/TCPlayer;component/Lib/About." + culture + ".txt", UriKind.Relative);
                else f = new Uri("/TCPlayer;component/Lib/" + fname, UriKind.Relative);
                var rs = Application.GetResourceStream(f);
                using (var sr = new StreamReader(rs.Stream))
                {
                    var content = sr.ReadToEnd();
                    AboutView.Text = content;
                }
            }
            catch (Exception)
            {
                LoadAbout("About.txt");
            }
        }

        public AboutDialog()
        {
            InitializeComponent();
            LoadAbout();
        }

        public Action OkClicked
        {
            get;
            set;
        }
    }
}
