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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WPFSoundVisualizationLib;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for WaveForm.xaml
    /// </summary>
    public partial class WaveForm : UserControl
    {
        private const int _updatePeriod = 20;
        private const float _datalength = (_updatePeriod * 2) / 1000.0f;
        private DispatcherTimer _visualTimer;
        private short[] channelData;
        private ISpectrumPlayer soundPlayer;

        private void _visualTimer_Tick(object sender, EventArgs e)
        {
            if (!Application.Current.MainWindow.IsActive) return;
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

        public WaveForm()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _visualTimer = new DispatcherTimer();
            _visualTimer.Interval = TimeSpan.FromMilliseconds(_updatePeriod);
            _visualTimer.Tick += _visualTimer_Tick;
        }

        public static double Map(short sample, double out_min, double out_max)
        {
            return (sample - short.MinValue) * (out_max - out_min) / (short.MaxValue - short.MinValue) + out_min;
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _visualTimer.Start();
            }
        }
    }
}
