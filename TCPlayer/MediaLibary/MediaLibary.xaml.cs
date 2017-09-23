using AppLib.WPF.MVVM;
using System.Windows.Controls;
using TCPlayer.MediaLibary.DB;

namespace TCPlayer.MediaLibary
{
    /// <summary>
    /// Interaction logic for MediaLibary.xaml
    /// </summary>
    public partial class MediaLibary : UserControl, IMediaLibary
    {
        public MediaLibary()
        {
            InitializeComponent();
        }

        public MediaLibaryViewModel ViewModel
        {
            get { return (MediaLibaryViewModel)DataContext; }
        }

        private void Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModelAction<MediaLibaryViewModel>(vm =>
            {
                vm.SelectedItems.Clear();
                foreach (var item in Data.SelectedItems)
                {
                    vm.SelectedItems.Add(item as TrackEntity);
                }
            });
        }

        public void Close()
        { }
    }
}
