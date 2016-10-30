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
        public delegate void EqualizerBandChange(int band, float value);

        private bool _fireevent;

        public event EqualizerBandChange EqBandChanged;

        public Equalizer()
        {
            InitializeComponent();
            _fireevent = true;
        }

        private void FreqChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_fireevent) return;
            var s = (sender as Slider).Name;
            int index = 0;
            switch (s)
            {
                case "Freq32":
                    index = 0;
                    break;
                case "Freq64":
                    index = 1;
                    break;
                case "Freq125":
                    index = 2;
                    break;
                case "Freq250":
                    index = 3;
                    break;
                case "Freq500":
                    index = 4;
                    break;
                case "Freq1k":
                    index = 5;
                    break;
                case "Freq2k":
                    index = 6;
                    break;
                case "Freq4k":
                    index = 7;
                    break;
                case "Freq8k":
                    index = 8;
                    break;
                case "Freq9k":
                    index = 9;
                    break;
            }

            if (EqBandChanged != null)
            {
                EqBandChanged(index, (float)e.NewValue);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _fireevent = false;
            var values = Helpers.LoadEqPresets();
            Freq32.Value = values[0];
            Freq64.Value = values[1];
            Freq125.Value = values[2];
            Freq250.Value = values[3];
            Freq500.Value = values[4];
            Freq1k.Value = values[5];
            Freq2k.Value = values[6];
            Freq4k.Value = values[7];
            Freq8k.Value = values[8];
            Freq16k.Value = values[9];
            _fireevent = true;
        }

        private void BtnPresetFlat_Click(object sender, RoutedEventArgs e)
        {
            Freq32.Value = 0.0d;
            Freq64.Value = 0.0d;
            Freq125.Value = 0.0d;
            Freq250.Value = 0.0d;
            Freq500.Value = 0.0d;
            Freq1k.Value = 0.0d;
            Freq2k.Value = 0.0d;
            Freq4k.Value = 0.0d;
            Freq8k.Value = 0.0d;
            Freq16k.Value = 0.0d;
        }
    }
}
