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
    }
}
