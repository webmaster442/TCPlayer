using AppLib.WPF.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCPlayer.MediaLibary.DB;
using AppLib.WPF.MVVM;

namespace TCPlayer
{
    /// <summary>
    /// Interaction logic for MediaWindow.xaml
    /// </summary>
    public partial class MediaWindow : CoolWindow
    {
        public MediaWindow()
        {
            InitializeComponent();
            Artists.ItemsSource = DataBase.Instance.DatabaseCache.Artists;
            Albums.ItemsSource = DataBase.Instance.DatabaseCache.Albums;
            Years.ItemsSource = DataBase.Instance.DatabaseCache.Years;
            Genres.ItemsSource = DataBase.Instance.DatabaseCache.Geneires;
        }

        private void CoolWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Collapsed;
        }

        private void Artists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = this.GetViewModel<MediaWindowViewModel>();
            var item = Artists.SelectedItem as string;
            vm.DoQuery(QueryInput.ArtistQuery(item));
        }

        private void Albums_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
