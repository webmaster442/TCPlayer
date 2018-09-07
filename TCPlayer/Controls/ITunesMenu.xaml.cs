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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Webmaster442.LibItunesXmlDb;
using System.Linq;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for ITunesMenu.xaml
    /// </summary>
    public partial class ITunesMenu : MenuItem
    {
        private ITunesXmlDb iTunes;

        public ITunesMenu()
        {
            InitializeComponent();
            MenuItunes_Loaded();
        }

        public event EventHandler<IEnumerable<string>> FilesProvidedEvent;

        private void MenuItunes_Loaded()
        {
            if (!ITunesXmlDb.UserHasItunesDb || DesignerProperties.GetIsInDesignMode(this))
            {
                IsEnabled = false;
                return;
            }
            try
            {
                ITunesXmlDbOptions options = new ITunesXmlDbOptions
                {
                    ExcludeNonExistingFiles = true,
                    ParalelParsingEnabled = true
                };
                iTunes = new ITunesXmlDb(ITunesXmlDb.UserItunesDbPath, options);
                CreateMenuItems(MenuAlbums, iTunes.Albums.Where(x => !string.IsNullOrEmpty(x)));
                CreateMenuItems(MenuArtists, iTunes.Artists.Where(x => !string.IsNullOrEmpty(x)));
                CreateMenuItems(MenuGenres, iTunes.Genres.Where(x => !string.IsNullOrEmpty(x)));
                CreateMenuItems(MenuYears, iTunes.Years.Where(x => !string.IsNullOrEmpty(x)));
                CreateMenuItems(MenuPlaylists, iTunes.Playlists.Where(x => !string.IsNullOrEmpty(x)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.Error_Title, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                IsEnabled = false;
                return;
            }
        }

        private void CreateMenuItems(MenuItem menuTarget, IEnumerable<string> items)
        {
            menuTarget.Items.Clear();
            foreach (var item in items)
            {
                MenuItem subitem = new MenuItem
                {
                    Tag = string.Copy(menuTarget.Tag as string),
                    Header = item
                };
                subitem.Click += Subitem_Click;
                menuTarget.Items.Add(subitem);
            }
        }

        private void Subitem_Click(object sender, RoutedEventArgs e)
        {
            if (FilesProvidedEvent != null)
            {
                if (sender is MenuItem s)
                {
                    IEnumerable<string> files = null;
                    string tag = s.Tag.ToString();
                    string content = s.Header.ToString();
                    try
                    {
                        switch (tag)
                        {
                            case "Albums":
                                files = iTunes.Filter(FilterKind.Album, content).Select(t => t.FilePath);
                                break;
                            case "Artists":
                                files = iTunes.Filter(FilterKind.Artist, content).Select(t => t.FilePath);
                                break;
                            case "Years":
                                files = iTunes.Filter(FilterKind.Year, content).Select(t => t.FilePath);
                                break;
                            case "Genres":
                                files = iTunes.Filter(FilterKind.Genre, content).Select(t => t.FilePath);
                                break;
                            case "Playlists":
                                files = iTunes.ReadPlaylist(content).Select(t => t.FilePath);
                                break;
                        }
                        FilesProvidedEvent?.Invoke(this, files);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Properties.Resources.Error_Title, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
