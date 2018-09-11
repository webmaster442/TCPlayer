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
using System.Windows;
using System.Windows.Controls;

namespace TCPlayer.Controls.Notification
{
    /// <summary>
    /// Interaction logic for NotificationPlaceSelector.xaml
    /// </summary>
    public partial class NotificationPlaceSelector : UserControl
    {
        public NotificationPlaceSelector()
        {
            InitializeComponent();
        }

        public NotificationPosition Position
        {
            get { return (NotificationPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(NotificationPosition), typeof(NotificationPlaceSelector), new PropertyMetadata(NotificationPosition.LeftTop, PositionChanged));

        private static void PositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as NotificationPlaceSelector;
            sender.SetSelection((int)e.NewValue);
        }

        private void SetSelection(int value)
        {
            foreach (RadioButton rb in SelectorGrid.Children)
            {
                if (Convert.ToInt32(rb.Tag) == value)
                {
                    rb.IsChecked = true;
                }
                else
                {
                    rb.IsChecked = false;
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            int ret = 0;
            foreach (RadioButton rb in SelectorGrid.Children)
            {
                if (rb.IsChecked == true)
                {
                    ret = Convert.ToInt32(rb.Tag);
                    break;
                }
            }
            Position = (NotificationPosition)ret;
        }
    }
}
