using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFSoundVisualizationLib;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for WaveForm.xaml
    /// </summary>
    public partial class WaveForm : UserControl
    {
        private DispatcherTimer _visualTimer;
        private ISpectrumPlayer soundPlayer;
        private short[] channelData;
        private const float _updatePeriod = 0.025f;


        public WaveForm()
        {
            InitializeComponent();
            _visualTimer = new DispatcherTimer();
            _visualTimer.Interval = TimeSpan.FromMilliseconds(_updatePeriod);
            _visualTimer.Tick += _visualTimer_Tick;
        }

        public void RegisterSoundPlayer(ISpectrumPlayer soundPlayer)
        {
            this.soundPlayer = soundPlayer;
            this.soundPlayer.PropertyChanged += soundPlayer_PropertyChanged;
            _visualTimer.Start();
        }

        public void UnRegisterSoundPlayer()
        {
            if (this.soundPlayer != null)
            {
                this.soundPlayer.PropertyChanged -= soundPlayer_PropertyChanged;
                this.soundPlayer = null;
            }
            _visualTimer.Stop();
        }

        private void soundPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsPlaying":
                    if (soundPlayer.IsPlaying && !_visualTimer.IsEnabled)
                        _visualTimer.Start();
                    else
                        _visualTimer.Stop();
                    break;
            }
        }

        private void _visualTimer_Tick(object sender, EventArgs e)
        {
            if (soundPlayer == null || RenderSize.Width < 1 || RenderSize.Height < 1 || !soundPlayer.IsPlaying)
                return;

            if (soundPlayer.IsPlaying && !soundPlayer.GetChannelData(out channelData, _updatePeriod))
                return;

            if (Visibility == Visibility.Collapsed)
                return;

            PolyLine.Points.Clear();

            int points = 100;
            int step = channelData.Length / points;
            double xscale = ActualWidth / channelData.Length;
            for (int i = 1; i < channelData.Length; i += step)
            {

                double x = i * xscale;
                double y = Map(channelData[i], 0.0d, ActualHeight) * 0.95;
                PolyLine.Points.Add(new Point(x, y));
            }
        }

        public static double Map(short sample,  double out_min, double out_max)
        {
            return (sample - short.MinValue) * (out_max - out_min) / (short.MaxValue - short.MinValue) + out_min;
        }
    }
}
