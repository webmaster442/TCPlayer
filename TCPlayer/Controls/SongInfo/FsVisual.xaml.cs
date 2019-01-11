using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WPFSoundVisualizationLib;

namespace TCPlayer.Controls.SongInfo
{
    /// <summary>
    /// Interaction logic for FsVisual.xaml
    /// </summary>
    public partial class FsVisual : Window
    {
        private const int _updatePeriod = 20;
        private const float _datalength = (_updatePeriod * 2) / 1000.0f;
        private DispatcherTimer _visualTimer;
        private short[] channelData;
        private ISpectrumPlayer soundPlayer;
        private bool _indesign;

        private void _visualTimer_Tick(object sender, EventArgs e)
        {
            if (!IsVisible) return;
            if (_indesign) return;
            UpdateWaveForm();
        }

        private void UpdateWaveForm()
        {
            if (soundPlayer == null || RenderSize.Width < 1 || RenderSize.Height < 1 || !soundPlayer.IsPlaying)
            {
                _visualTimer.Stop();
                return;
            }

            if (soundPlayer.IsPlaying && !soundPlayer.GetChannelData(out channelData, _datalength))
            {
                _visualTimer.Stop();
                return;
            }

            if (Visibility == Visibility.Collapsed)
            {
                _visualTimer.Stop();
                return;
            }

            CircleVis.WaveData = channelData;
        }

        public FsVisual()
        {
            InitializeComponent();
            _indesign = DesignerProperties.GetIsInDesignMode(this);
            if (_indesign) return;

            _visualTimer = new DispatcherTimer();
            _visualTimer.Interval = TimeSpan.FromMilliseconds(_updatePeriod);
            _visualTimer.Tick += _visualTimer_Tick;
        }

        private void soundPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsPlaying":
                    if (soundPlayer.IsPlaying && !_visualTimer.IsEnabled)
                        _visualTimer.Start();
                    //else
                    //_visualTimer.Stop();
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = Math.Min(ActualWidth * 0.8, ActualHeight * 0.8);
            CircleVis.Width = size;
            CircleVis.Height = size;
            CircleVis.InvalidateVisual();
        }

        public void RegisterSoundPlayer(ISpectrumPlayer soundPlayer)
        {
            if (soundPlayer != null)
            {
                this.soundPlayer = soundPlayer;
                this.soundPlayer.PropertyChanged += soundPlayer_PropertyChanged;
                _visualTimer.Start();
            }
        }

        internal void UpdateMetaData(string text, ImageSource cover)
        {
            Info.Text = text;
            Filler.Source = cover;
            SmallCover.Source = cover;
        }
    }
}
