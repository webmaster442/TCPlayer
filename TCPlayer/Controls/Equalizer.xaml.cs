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
using TCPlayer.Code;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for Equalizer.xaml
    /// </summary>
    public partial class Equalizer : UserControl
    {
        private void BtnBass_Click(object sender, RoutedEventArgs e)
        {
            SetSliders(0.0d, 0.0d, 6.5d);
        }

        private void BtnHigh_Click(object sender, RoutedEventArgs e)
        {
            SetSliders(6.5d, 0.0, 0.0d);
        }

        private void BtnMid_Click(object sender, RoutedEventArgs e)
        {
            SetSliders(0.0d, 6.5d, 0.0d);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            SetSliders(0.0d, 0.0d, 0.0d);
        }

        private void SetSliders(double s0, double s1, double s2)
        {
            if (EqSliderChange != null)
            {
                Slider0.Value = s0;
                Slider1.Value = s1;
                Slider2.Value = s2;
                EqSliderChange.Invoke(this, new RoutedEventArgs());
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EqSliderChange?.Invoke(this, new RoutedEventArgs());
        }

        public event RoutedEventHandler EqSliderChange;

        public Equalizer()
        {
            InitializeComponent();
        }

        public EqConfig EqConfiguration
        {
            get
            {
                return new EqConfig
                {
                    Bass = Slider2.Value,
                    Mid = Slider1.Value,
                    Treble = Slider0.Value
                };
            }
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.EqBass = Slider2.Value;
            Properties.Settings.Default.EqMid = Slider1.Value;
            Properties.Settings.Default.EqHigh = Slider0.Value;
        }

        internal void LoadSettings()
        {
            SetSliders(Properties.Settings.Default.EqHigh, 
                       Properties.Settings.Default.EqMid, 
                       Properties.Settings.Default.EqBass);
        }
    }
}
