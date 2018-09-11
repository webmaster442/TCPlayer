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
