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

        private void Data_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.ViewModelAction<MediaWindowViewModel>(vm =>
            {
                vm.SelectedTrack = Data.SelectedItem as TrackEntity;
            });
        }

        private void Data_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ViewModelAction<MediaWindowViewModel>(vm =>
            {
                vm.PlaySelectedTrackCommand.Execute(null);
            });
        }
    }
}
