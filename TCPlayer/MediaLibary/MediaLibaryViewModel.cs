using AppLib.Common.Extensions;
using AppLib.WPF.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TCPlayer.MediaLibary.DB;

namespace TCPlayer.MediaLibary
{
    public partial class MediaLibaryViewModel: ViewModel<IMediaLibary>
    {
        public DelegateCommand FilterArtstsCommand { get; private set; }
        public DelegateCommand FilterAlbumsCommand { get; private set; }
        public DelegateCommand FilterYearsCommand { get; private set; }
        public DelegateCommand FilterGenreCommand { get; private set; }
        public DelegateCommand FilterQueryCommand { get; private set; }

        public DelegateCommand ListQueryCommand { get; private set; }

        public DelegateCommand MenuAddFilesCommand { get; private set; }
        public DelegateCommand MenuSendToPlaylistCommand { get; private set; }
        public DelegateCommand MenuCreateQueryCommand { get; private set; }
        public DelegateCommand MenuBackupDbCommand { get; private set; }

        private ListingType _listing;

        public ObservableCollection<TrackEntity> DisplayItems { get; private set; }
        public ObservableCollection<string> ListItems { get; private set; }
        
        public ListingType ListingType
        {
            get { return _listing; }
            set { SetValue(ref _listing, value); }
        }

        public MediaLibaryViewModel()
        {
            DisplayItems = new ObservableCollection<TrackEntity>();
            ListItems = new ObservableCollection<string>();
            FilterArtstsCommand = DelegateCommand.ToCommand(FilterArtsts);
            FilterAlbumsCommand = DelegateCommand.ToCommand(FilterAlbums);
            FilterYearsCommand = DelegateCommand.ToCommand(FilterYears);
            FilterGenreCommand = DelegateCommand.ToCommand(FilterGenre);
            FilterQueryCommand = DelegateCommand.ToCommand(FilterQuery);

            ListQueryCommand = DelegateCommand.ToCommand(ListQuery);

            MenuAddFilesCommand = DelegateCommand.ToCommand(MenuAddFiles);
            MenuSendToPlaylistCommand = DelegateCommand.ToCommand(MenuSendToPlaylist);
            MenuCreateQueryCommand = DelegateCommand.ToCommand(MenuCreateQuery);
            MenuBackupDbCommand = DelegateCommand.ToCommand(MenuBackupDb);
        }

        private void FilterQuery()
        {
            throw new NotImplementedException();
        }

        private void FilterGenre()
        {
            ListingType = ListingType.Genre;
            ListItems = DataBase.Instance.DatabaseCache.Geneires;
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
            ListItems = DataBase.Instance.DatabaseCache.Albums;
        }

        private void FilterArtsts()
        {
            ListingType = ListingType.Artists;
            ListItems = DataBase.Instance.DatabaseCache.Artists;
        }

        private void DoQuery(QueryInput queryInput)
        {
            var items = DataBase.Instance.Execute(queryInput);
            DisplayItems.Clear();
            DisplayItems.AddRange(items);
        }

        private void ListQuery()
        {
            switch (ListingType)
            {
                case ListingType.Albums:
                    break;
                case ListingType.Artists:
                    break;
                case ListingType.Genre:
                    break;
                case ListingType.Years:
                    break;
                case ListingType.Query:
                    break;
            }
        }
    }
}
