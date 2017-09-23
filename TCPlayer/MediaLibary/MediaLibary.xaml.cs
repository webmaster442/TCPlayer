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
