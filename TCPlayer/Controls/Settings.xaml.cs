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
using System.Windows;
using System.Windows.Controls;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        private void BtnClearHistory_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RecentURLs = "";
            App.RecentUrls.Clear();
            MessageBox.Show(Properties.Resources.Settings_UrlHistoryCleared,
                            Properties.Resources.Settings_UrlHistoryClearTitle,
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSoundFont_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Sound Fonts|*.sf2;*.sfz";
            dialog.Multiselect = false;
            dialog.Title = Properties.Resources.Settings_SelectSoundFontTitle;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.SoundfontPath = dialog.FileName;
            }
        }

        public Settings()
        {
            InitializeComponent();
        }
    }
}
