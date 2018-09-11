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
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace TCPlayer.Controls.Network
{
    /// <summary>
    /// Interaction logic for NetworkMenu.xaml
    /// </summary>
    public partial class NetworkMenu : Menu
    {
        public ObservableCollection<NetworkSearchProvider> Providers
        {
            get;
            private set;
        }

        public SearchCommand SearchArtist
        {
            get;
            private set;
        }

        public SearchCommand SearchSong
        {
            get;
            private set;
        }

        public string Artist
        {
            get { return SearchArtist.UrlParameter; }
            set { SearchArtist.UrlParameter = value; }
        }

        public string Song
        {
            get { return SearchSong.UrlParameter; }
            set { SearchSong.UrlParameter = value; }
        }

        public NetworkMenu()
        {
            InitializeComponent();
            Providers = new ObservableCollection<NetworkSearchProvider>(Constants.NetworkProviders);
            SearchArtist = new SearchCommand();
            SearchSong = new SearchCommand();
            DataContext = this;
            NetMenu.DataContext = this;
        }
    }
}
