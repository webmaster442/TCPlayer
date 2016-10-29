using System.Windows;
using System.Windows.Controls;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for Equalizer.xaml
    /// </summary>
    public partial class Equalizer : UserControl
    {
        private float[] _levels;

        public delegate void EqualizerBandChange(int band, float value);

        public event EqualizerBandChange EqBandChanged;

        public Equalizer()
        {
            InitializeComponent();
        }

        private void FreqChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
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
    }
}
