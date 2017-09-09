using AppLib.WPF.Controls;
using AppLib.WPF.MVVM;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TCPlayer.MediaLibary.DB;

namespace TCPlayer
{
    /// <summary>
    /// Interaction logic for MediaWindow.xaml
    /// </summary>
    public partial class MediaWindow : CoolWindow, IMediaWindowView
    {

        public MediaWindow()
        {
            InitializeComponent();
            Artists.ItemsSource = DataBase.Instance.DatabaseCache.Artists;
            Albums.ItemsSource = DataBase.Instance.DatabaseCache.Albums;
            Years.ItemsSource = DataBase.Instance.DatabaseCache.Years;
            Genres.ItemsSource = DataBase.Instance.DatabaseCache.Geneires;
            this.GetViewModel<MediaWindowViewModel>().View = this;
        }

        public bool IsProgressVisible
        {
            get { return ProgressIndicator.Visibility == Visibility.Visible; }
            set { ProgressIndicator.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;  }
        }

        private void CoolWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Collapsed;
        }

        private void Artists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ViewModelAction<MediaWindowViewModel>(vm => 
            {
                var item = Artists.SelectedItem as string;
                vm.DoQuery(QueryInput.ArtistQuery(item));
            });
        }

        private void Albums_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ViewModelAction<MediaWindowViewModel>(vm =>
            {
                var item = Albums.SelectedItem as string;
                vm.DoQuery(QueryInput.AlbumQuery(item));
            });
        }

        private void Years_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ViewModelAction<MediaWindowViewModel>(vm =>
            {
                var item = Convert.ToUInt32(Years.SelectedItem);
                vm.DoQuery(QueryInput.YearQuery(item));
            });
        }

        private void Genres_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ViewModelAction<MediaWindowViewModel>(vm =>
            {
                var item = Genres.SelectedItem as string;
                vm.DoQuery(QueryInput.GenerireQuery(item));
            });
        }

        private void Saved_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
