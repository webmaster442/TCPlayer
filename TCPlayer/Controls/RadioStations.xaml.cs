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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using TCPlayer.Code;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for RadioStations.xaml
    /// </summary>
    public partial class RadioStations : UserControl
    {
        private bool loaded;

        private void FileSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (FileSelector.SelectedIndex)
            {
                case 0:
                    LoadXML("radiotray.xml");
                    break;
                case 1:
                    LoadXML("radiotray.hu.xml");
                    break;
            }
        }

        private void LoadXML(string file)
        {
            if (!loaded) return;
            var nodes = new List<TreeViewItem>();
            var rs = Application.GetResourceStream(new Uri("/TCPlayer;component/Lib/" + file, UriKind.Relative));
            using (var stream = rs.Stream)
            {
                var xs = new XmlSerializer(typeof(RadioGroup), new XmlRootAttribute("bookmarks"));
                var data = (RadioGroup)xs.Deserialize(stream);
                var root = data.SubGroups[0].SubGroups;
                foreach (var g in root)
                {
                    nodes.Add(RenderNodes(g));
                }
            }
            RadioView.ItemsSource = nodes;
        }

        private void RadioView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemDoubleClcik != null)
            {
                var item = RadioView.SelectedItem as TreeViewItem;
                if (item == null) return;
                SelectedUrl = item.ToolTip?.ToString();
                if (!string.IsNullOrEmpty(SelectedUrl))
                    ItemDoubleClcik(sender, null);
            }
        }

        private TreeViewItem RenderNodes(RadioGroup group)
        {
            TreeViewItem ret = new TreeViewItem();
            ret.Header = group.Name;
            if (group.SubGroups.Count > 0)
            {
                foreach (var sub in group.SubGroups)
                {
                    var child = RenderNodes(sub);
                    ret.Items.Add(child);
                }
            }
            foreach (var station in group.Stations)
            {
                var child = new TreeViewItem();
                child.Header = station.Name;
                child.ToolTip = station.Url;
                ret.Items.Add(child);
            }
            return ret;
        }

        public event RoutedEventHandler ItemDoubleClcik;

        public RadioStations()
        {
            InitializeComponent();
            loaded = true;
            FileSelector.SelectedIndex = 0;
        }

        public string SelectedUrl
        {
            get;
            private set;
        }
    }
}
