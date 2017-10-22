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
using AppLib.Common.Extensions;
using AppLib.WPF.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TCPlayer.MediaLibary.DB;

namespace TCPlayer.MediaLibary
{
    public partial class MediaLibaryViewModel: ViewModel<IMediaLibaryView>
    {
        public DelegateCommand FilterArtstsCommand { get; private set; }
        public DelegateCommand FilterAlbumsCommand { get; private set; }
        public DelegateCommand FilterYearsCommand { get; private set; }
        public DelegateCommand FilterGenreCommand { get; private set; }
        public DelegateCommand FilterQueryCommand { get; private set; }

        public DelegateCommand<string> ListQueryCommand { get; private set; }

        public DelegateCommand MenuAddFilesCommand { get; private set; }
        public DelegateCommand MenuAddFolderCommand { get; private set; }
        public DelegateCommand MenuSendToPlaylistCommand { get; private set; }
        public DelegateCommand MenuCreateQueryCommand { get; private set; }
        public DelegateCommand MenuBackupDbCommand { get; private set; }

        private ListingType _listing;

        public ObservableCollection<TrackEntity> DisplayItems { get; private set; }
        public ObservableCollection<string> ListItems { get; private set; }
        public ObservableCollection<TrackEntity> SelectedItems { get; private set; }
        
        public ListingType ListingType
        {
            get { return _listing; }
            set { SetValue(ref _listing, value); }
        }

        public MediaLibaryViewModel(IMediaLibaryView mediaLibary): base(mediaLibary)
        {
            DisplayItems = new ObservableCollection<TrackEntity>();
            ListItems = new ObservableCollection<string>();
            SelectedItems = new ObservableCollection<TrackEntity>();
            FilterArtstsCommand = DelegateCommand.ToCommand(FilterArtsts);
            FilterAlbumsCommand = DelegateCommand.ToCommand(FilterAlbums);
            FilterYearsCommand = DelegateCommand.ToCommand(FilterYears);
            FilterGenreCommand = DelegateCommand.ToCommand(FilterGenre);
            FilterQueryCommand = DelegateCommand.ToCommand(FilterQuery);

            ListQueryCommand = DelegateCommand<string>.ToCommand(ListQuery);

            MenuAddFilesCommand = DelegateCommand.ToCommand(MenuAddFiles);
            MenuAddFolderCommand = DelegateCommand.ToCommand(MenuAddFolder);
            MenuSendToPlaylistCommand = DelegateCommand.ToCommand(MenuSendToPlaylist);
            MenuCreateQueryCommand = DelegateCommand.ToCommand(MenuCreateQuery);
            MenuBackupDbCommand = DelegateCommand.ToCommand(MenuBackupDb);
        }

        private void FilterQuery()
        {
            ListingType = ListingType.Query;
        }

        private void FilterGenre()
        {
            ListingType = ListingType.Genre;
            ListItems.Clear();
            ListItems.AddRange(DataBase.Instance.DatabaseCache.Geneires);
        }

        private void FilterYears()
        {
            ListingType = ListingType.Years;
            ListItems.Clear();
            ListItems.AddRange(DataBase.Instance.DatabaseCache.Years.Select(i => i.ToString()));
        }

        private void FilterAlbums()
        {
            ListingType = ListingType.Albums;
            ListItems.Clear();
            ListItems.AddRange(DataBase.Instance.DatabaseCache.Albums);
        }

        private void FilterArtsts()
        {
            ListingType = ListingType.Artists;
            ListItems.Clear();
            ListItems.AddRange(DataBase.Instance.DatabaseCache.Artists);
        }

        private void DoQuery(QueryInput queryInput)
        {
            var items = DataBase.Instance.Execute(queryInput);
            DisplayItems.Clear();
            DisplayItems.AddRange(items);
        }

        private void ListQuery(string selecteditem)
        {
            if (selecteditem == null) return;

            switch (ListingType)
            {
                case ListingType.Albums:
                    DoQuery(QueryInput.AlbumQuery(selecteditem));
                    break;
                case ListingType.Artists:
                    DoQuery(QueryInput.ArtistQuery(selecteditem));
                    break;
                case ListingType.Genre:
                    DoQuery(QueryInput.GenerireQuery(selecteditem));
                    break;
                case ListingType.Years:
                    DoQuery(QueryInput.YearQuery(Convert.ToUInt32(selecteditem)));
                    break;
                case ListingType.Query:
                    
                    break;
            }
        }
    }
}
