using AppLib.WPF.MVVM;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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


        private void Data_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void Data_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        public void Close()
        { }
    }
}
