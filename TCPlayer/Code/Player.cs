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
using ManagedBass;
using ManagedBass.Cd;
using ManagedBass.Fx;
using ManagedBass.Midi;
using ManagedBass.Mix;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using TCPlayer.Properties;
using WPFSoundVisualizationLib;

namespace TCPlayer.Code
{
    internal class Player : IDisposable, ISpectrumPlayer, INotifyPropertyChanged
    {
        private static readonly Player _instance = new Player();

        private readonly int _maxfft;

        private DownloadProcedure _callback;

        private bool _initialized;

        private bool _isplaying;

        private bool _isstream;

        private float _lastvol;

        private bool _paused;

        private int _source, _mixer, _fx;

        PeakEQParameters _eq;
        GCHandle _handle;
        private EqConfig _eqConfig;

        private Player()
        {
            var enginedir = AppDomain.CurrentDomain.BaseDirectory;
            if (Is64Bit) enginedir = Path.Combine(enginedir, @"Engine\x64");
            else enginedir = Path.Combine(enginedir, @"Engine\x86");
            Bass.Load(enginedir);
            BassMix.Load(enginedir);
            BassCd.Load(enginedir);
            BassFx.Load(enginedir);
            Bass.PluginLoad(enginedir + "\\bass_aac.dll");
            Bass.PluginLoad(enginedir + "\\bass_ac3.dll");
            Bass.PluginLoad(enginedir + "\\bass_ape.dll");
            Bass.PluginLoad(enginedir + "\\bass_mpc.dll");
            Bass.PluginLoad(enginedir + "\\bass_spx.dll");
            Bass.PluginLoad(enginedir + "\\bass_tta.dll");
            Bass.PluginLoad(enginedir + "\\bassalac.dll");
            Bass.PluginLoad(enginedir + "\\bassdsd.dll");
            Bass.PluginLoad(enginedir + "\\bassflac.dll");
            Bass.PluginLoad(enginedir + "\\bassopus.dll");
            Bass.PluginLoad(enginedir + "\\basswma.dll");
            Bass.PluginLoad(enginedir + "\\basswv.dll");
            Bass.PluginLoad(enginedir + "\\bassmidi.dll");
            _callback = MyDownloadProc;
            _maxfft = (int)(DataFlags.Available | DataFlags.FFT2048);
            _eq = new PeakEQParameters();
            _handle = GCHandle.Alloc(_eq, GCHandleType.Pinned);

        }

        /// <summary>
        /// Display Error message
        /// </summary>
        /// <param name="message"></param>
        private void Error(string message)
        {
            var error = Bass.LastError;
            string text = string.Format(Resources.Engine_Error, message, (int)error, error);
            throw new Exception(text);
        }

        private void MyDownloadProc(IntPtr buffer, int length, IntPtr user)
        {
            var ptr = Bass.ChannelGetTags(_source, TagType.META);
            if (ptr != IntPtr.Zero)
            {
                var array = Native.IntPtrToArray(ptr);
                if (array != null && MetaChanged != null)
                    MetaChanged(this, PtocessTags(array));
            }
            else
            {
                ptr = Bass.ChannelGetTags(_source, TagType.OGG);
                if (ptr != null)
                {
                    var array = Native.IntPtrToArray(ptr);
                    if (array != null && MetaChanged != null)
                        MetaChanged(this, PtocessTags(array, true));
                }
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private string PtocessTags(string[] array, bool icecast = false)
        {
            string ret = "";
            if (icecast)
            {
                foreach (var item in array)
                {
                    if (item.StartsWith("ARTIST")) ret += item.Replace("ARTIST=", "");
                    else if (item.StartsWith("TITLE")) ret += item.Replace("TITLE=", " - ");
                    else continue;
                }
            }
            else
            {
                var contents = array[0].Split(';');
                foreach (var item in contents)
                {
                    if (item.StartsWith("StreamTitle='")) ret += item.Replace("StreamTitle='", "").Replace("'", "");
                    else continue;
                }
            }
            return ret;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_source != 0)
            {
                if (_mixer != 0)
                {
                    Stop();
                }
                RemoveEq(ref _mixer);
                Bass.StreamFree(_source);
                Bass.MusicFree(_source);
                Bass.StreamFree(_mixer);
                _mixer = 0;
                _source = 0;
            }
            if (_handle.IsAllocated) _handle.Free();
            if (_initialized) Bass.Free();
            BassCd.Unload();
            BassFx.Unload();
            BassMix.Unload();
            Bass.PluginFree(0);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<string> MetaChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Player Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Currently used device index. Used for setting saving
        /// </summary>
        public int CurrentDeviceID
        {
            get;
            private set;
        }

        public bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }
        /// <summary>
        /// IsPaused state
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return _paused || (_mixer == 0);
            }
        }

        public bool IsPlaying
        {
            get { return _isplaying; }
            set
            {
                if (value == _isplaying) return;
                _isplaying = value;
                NotifyPropertyChanged(nameof(IsPlaying));
            }
        }

        public bool IsStream
        {
            get { return _isstream; }
        }

        /// <summary>
        /// Gets the channel length
        /// </summary>
        public double Length
        {
            get
            {
                var len = Bass.ChannelGetLength(_source);
                return Bass.ChannelBytes2Seconds(_source, len);
            }
        }

        /// <summary>
        /// Returns mixer handle
        /// </summary>
        public int MixerHandle
        {
            get { return _mixer; }
        }

        public double Position
        {
            get
            {
                var pos = Bass.ChannelGetPosition(_source);
                return Bass.ChannelBytes2Seconds(_source, pos);
            }
            set
            {
                var pos = Bass.ChannelSeconds2Bytes(_source, value);
                Bass.ChannelSetPosition(_source, pos);
            }
        }

        public EqConfig EqConfig
        {
            get { return _eqConfig; }
            set
            {
                _eqConfig = value;
                UpdateFxConfiguration(_eqConfig);
            }
        }

        /// <summary>
        /// Player source handle
        /// </summary>
        public int SourceHandle
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets or sets the Channel volume
        /// </summary>
        public float Volume
        {
            get
            {

                float temp = 0.0f;
                Bass.ChannelGetAttribute(_mixer, ChannelAttribute.Volume, out temp);
                return temp;
            }
            set
            {
                Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, value);
                _lastvol = value;
            }
        }

        /// <summary>
        /// List tracks on a CD drive
        /// </summary>
        /// <param name="drive">CD drive path</param>
        /// <returns>An array of playlist entry's</returns>
        public static string[] GetCdInfo(string drive)
        {
            var list = new List<string>();

            int drivecount = BassCd.DriveCount;
            int driveindex = 0;
            for (int i = 0; i < drivecount; i++)
            {

                var info = BassCd.GetInfo(i);
                if (info.DriveLetter == drive[0])
                {
                    driveindex = i;
                    break;
                }
            }

            if (BassCd.IsReady(driveindex))
            {
                var numtracks = BassCd.GetTracks(driveindex);
                var discid = BassCd.GetID(0, CDID.CDDB); //cddb connect
                if (App.DiscID != discid)
                {
                    var datas = BassCd.GetIDText(driveindex);
                    App.DiscID = discid;
                    App.CdData.Clear();
                    foreach (var data in datas)
                    {
                        var item = data.Split('=');
                        App.CdData.Add(item[0], item[1]);
                    }
                }
                for (int i = 0; i < numtracks; i++)
                {
                    var entry = string.Format("cd://{0}/{1}", driveindex, i);
                    list.Add(entry);
                }
            }
            BassCd.Release(driveindex);
            return list.ToArray();
        }

        /// <summary>
        /// Change output device
        /// </summary>
        /// <param name="name">string device</param>
        public void ChangeDevice(string name = null)
        {
            if (name == null)
            {
                CurrentDeviceID = -1;
                if (Properties.Settings.Default.SaveDevice)
                    CurrentDeviceID = Properties.Settings.Default.DeviceID;

                _initialized = Bass.Init(CurrentDeviceID, 48000, DeviceInitFlags.Frequency, IntPtr.Zero);
                if (!_initialized)
                {
                    Properties.Settings.Default.DeviceID = 0;
                    Properties.Settings.Default.Save();
                    Error(Resources.Engine_ErrorBass);
                    return;
                }
                Bass.Start();
            }
            for (int i = 0; i < Bass.DeviceCount; i++)
            {
                var device = Bass.GetDeviceInfo(i);
                if (device.Name == name)
                {
                    if (_initialized)
                    {
                        Bass.Free();
                        _initialized = false;
                    }

                    _initialized = Bass.Init(i, Settings.Default.SampleRate, DeviceInitFlags.Frequency, IntPtr.Zero);
                    CurrentDeviceID = i;
                    if (!_initialized)
                    {
                        Error(Resources.Engine_ErrorBass);
                        return;
                    }
                    Bass.Start();
                }
            }
        }

        /// <summary>
        /// Free used resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public bool GetChannelData(out short[] data, float seconds)
        {
            var length = (int)Bass.ChannelSeconds2Bytes(_mixer, seconds);
            if (length > 0)
            {
                data = new short[length / 2];
                length = Bass.ChannelGetData(_mixer, data, length);
                return true;
            }
            data = null;
            return false;
        }

        /// <summary>
        /// Gets the available output devices
        /// </summary>
        /// <returns>device names in an array</returns>
        public string[] GetDevices()
        {
            List<string> _devices = new List<string>(Bass.DeviceCount);
            for (int i = 1; i < Bass.DeviceCount; i++)
            {
                var device = Bass.GetDeviceInfo(i);
                if (device.IsEnabled) _devices.Add(device.Name);
            }
            return _devices.ToArray();
        }

        public bool GetFFTData(float[] fftDataBuffer)
        {
            return Bass.ChannelGetData(_mixer, fftDataBuffer, _maxfft) > 0;
        }

        public int GetFFTFrequencyIndex(int frequency)
        {
            var length = (int)FFTDataSize.FFT2048;
            int num = (int)Math.Round((double)length * (double)frequency / (double)Properties.Settings.Default.SampleRate);
            if (num > length / 2 - 1) num = length / 2 - 1;
            return num;
        }

        public void GetPlaybackStateNotification()
        {
            NotifyPropertyChanged(nameof(IsPlaying));
        }

        /// <summary>
        /// Load a file for playback
        /// </summary>
        /// <param name="file">File to load</param>
        public void Load(string file)
        {
            _isstream = false;
            if (Helpers.IsMidi(file))
            {
                if (!File.Exists(Properties.Settings.Default.SoundfontPath))
                {
                    Error(Resources.Engine_ErrorSoundfont);
                    return;
                }
                BassMidi.DefaultFont = Properties.Settings.Default.SoundfontPath;
            }
            if (_source != 0)
            {
                Bass.StreamFree(_source);
                _source = 0;
                IsPlaying = false;
            }
            if (_mixer != 0)
            {
                RemoveEq(ref _mixer);
                Bass.StreamFree(_mixer);
                _mixer = 0;
                IsPlaying = false;
            }
            var sourceflags = BassFlags.Decode | BassFlags.Loop | BassFlags.Float | BassFlags.Prescan;
            var mixerflags = BassFlags.MixerDownMix | BassFlags.MixerPositionEx | BassFlags.AutoFree;

            if (file.StartsWith("http://") || file.StartsWith("https://"))
            {
                _source = Bass.CreateStream(file, 0, sourceflags, _callback, IntPtr.Zero);
                _isstream = true;
                App.RecentUrls.Add(file);
            }
            else if (file.StartsWith("cd://"))
            {
                string[] info = file.Replace("cd://", "").Split('/');
                _source = BassCd.CreateStream(Convert.ToInt32(info[0]), Convert.ToInt32(info[1]), sourceflags);
            }
            else if (Helpers.IsTracker(file))
            {
                _source = Bass.MusicLoad(file, 0, 0, sourceflags);
            }
            else
            {
                _source = Bass.CreateStream(file, 0, 0, sourceflags);
            }
            if (_source == 0)
            {
                Error("Load failed");
                IsPlaying = false;
                _isstream = false;
                return;
            }
            var ch = Bass.ChannelGetInfo(_source);
            _mixer = BassMix.CreateMixerStream(ch.Frequency, ch.Channels, mixerflags);
            if (_mixer == 0)
            {
                Error(Resources.Engine_ErrorMixer);
                IsPlaying = false;
                return;
            }
            if (!BassMix.MixerAddChannel(_mixer, _source, BassFlags.MixerDownMix))
            {
                Error(Resources.Engine_ErrorMixerChannel);
                IsPlaying = false;
                return;
            }
            Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, _lastvol);
            InitEq(ref _mixer);
            Bass.ChannelPlay(_mixer, false);
            _paused = false;
            IsPlaying = true;
            NotifyPropertyChanged(nameof(IsPlaying));
        }

        /// <summary>
        /// Play / Pause
        /// </summary>
        public void PlayPause()
        {
            if (_paused)
            {
                Bass.ChannelPlay(_mixer, false);
                IsPlaying = true;
                _paused = false;
            }
            else
            {
                Bass.ChannelPause(_mixer);
                _paused = true;
                IsPlaying = false;
            }
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            Bass.ChannelStop(_mixer);
            IsPlaying = false;
            _paused = false;
        }

        public void VolumeValues(out int left, out int right)
        {
            left = Bass.ChannelGetLevelLeft(_mixer);
            right = Bass.ChannelGetLevelRight(_mixer);
        }

        private void UpdateFxConfiguration(EqConfig eqConfig)
        {
            for (int band = 0; band < 3; ++band)
            {
                _eq.lBand = band;
                Bass.FXGetParameters(_fx, _handle.AddrOfPinnedObject());
                _eq.fGain = eqConfig[band];
                Bass.FXSetParameters(_fx, _handle.AddrOfPinnedObject());
            }
        }

        private void RemoveEq(ref int chHandle)
        {
            Bass.ChannelRemoveFX(chHandle, _fx);
        }

        private void InitEq(ref int chHandle, float fGain = 0.0f)
        {
            if (_eqConfig == null) _eqConfig = new EqConfig();

            // set peaking equalizer effect with no bands
            _fx = Bass.ChannelSetFX(chHandle, EffectType.PeakEQ, 0); // BASS_ChannelSetFX(chan, BASS_FX_BFX_PEAKEQ, 0);

            _eq.fGain = fGain;
            _eq.fQ = EqConstants.fQ;
            _eq.fBandwidth = EqConstants.fBandwidth;
            _eq.lChannel = FXChannelFlags.All;

            // create 1st band for bass
            _eq.lBand = 0;
            _eq.fCenter = EqConstants.fCenter_Bass;
            Bass.FXSetParameters(_fx, _handle.AddrOfPinnedObject());

            // create 2nd band for mid
            _eq.lBand = 1;
            _eq.fCenter = EqConstants.fCenter_Mid;
            Bass.FXSetParameters(_fx, _handle.AddrOfPinnedObject());

            // create 3rd band for treble
            _eq.lBand = 2;
            _eq.fCenter = EqConstants.fCenter_Treble;
            Bass.FXSetParameters(_fx, _handle.AddrOfPinnedObject());

            UpdateFxConfiguration(_eqConfig);
        }
    }
}
