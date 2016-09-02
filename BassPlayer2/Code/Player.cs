using ManagedBass;
using ManagedBass.Cd;
using ManagedBass.Mix;
using System;
using System.Collections.Generic;
using System.IO;

namespace BassPlayer2.Code
{
    internal class Player : IDisposable
    {
        private bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        private bool _initialized;
        private int _source, _mixer;
        private float _lastvol;
        private bool _paused;

        public Player()
        {
            var enginedir = AppDomain.CurrentDomain.BaseDirectory;
            if (Is64Bit) enginedir = Path.Combine(enginedir, @"Engine\x64");
            else enginedir = Path.Combine(enginedir, @"Engine\x86");
            Bass.Load(enginedir);
            BassMix.Load(enginedir);
            BassCd.Load(enginedir);
            Bass.PluginLoad(enginedir + "\\bass_aac.dll");
            Bass.PluginLoad(enginedir + "\\bass_ac3.dll");
            Bass.PluginLoad(enginedir + "\\bass_alac.dll");
            Bass.PluginLoad(enginedir + "\\bassflac.dll");
            Bass.PluginLoad(enginedir + "\\basswma.dll");
            Bass.PluginLoad(enginedir + "\\basswv.dll");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_initialized) Bass.Free();
            BassCd.Unload();
            BassMix.Unload();
            Bass.PluginFree(0);
            Bass.Unload();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Free used resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Display Error message
        /// </summary>
        /// <param name="message"></param>
        private void Error(string message)
        {
            var error = Bass.LastError;
            string text = string.Format("{0}\r\nBass Error code: {1}\r\nError Description: {2}", message, (int)error, error);
            throw new Exception(text);
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
        /// Load a file for playback
        /// </summary>
        /// <param name="file">File to load</param>
        public void Load(string file)
        {
            if (_source != 0)
            {
                Bass.StreamFree(_source);
                _source = 0;
            }
            if (_mixer != 0)
            {
                Bass.StreamFree(_mixer);
                _mixer = 0;
            }
            var sourceflags = BassFlags.Decode | BassFlags.Loop | BassFlags.Float | BassFlags.Prescan;
            var mixerflags = BassFlags.MixerDownMix | BassFlags.MixerPositionEx | BassFlags.AutoFree;

            if (file.StartsWith("http://") || file.StartsWith("https://"))
            {
                _source = Bass.CreateStream(file, 0, sourceflags, null);
            }
            else if (file.StartsWith("cd://"))
            {
                string[] info = file.Replace("cd://", "").Split('/');
                _source = BassCd.CreateStream(Convert.ToInt32(info[0]), Convert.ToInt32(info[1]), sourceflags);
            }
            else
            {
                _source = Bass.CreateStream(file, 0, 0, sourceflags);
            }
            if (_source == 0)
            {
                Error("Load failed");
                return;
            }
            var ch = Bass.ChannelGetInfo(_source);
            _mixer = BassMix.CreateMixerStream(ch.Frequency, ch.Channels, mixerflags);
            if (_mixer == 0)
            {
                Error("Mixer stream create failed");
                return;
            }
            if (!BassMix.MixerAddChannel(_mixer, _source, BassFlags.MixerDownMix))
            {
                Error("Mixer chanel adding failed");
                return;
            }
            Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, _lastvol);
            _paused = false;
        }

        /// <summary>
        /// Returns mixer handle
        /// </summary>
        public int MixerHandle
        {
            get { return _mixer; }
        }

        /// <summary>
        /// Play
        /// </summary>
        public void Play()
        {
            _paused = false;
            Bass.ChannelPlay(_mixer, false);
        }

        /// <summary>
        /// Pause
        /// </summary>
        public void Pause()
        {
            _paused = true;
            Bass.ChannelPause(_mixer);
        }

        /// <summary>
        /// Play / Pause
        /// </summary>
        public void PlayPause()
        {
            if (_paused)
            {
                Bass.ChannelPlay(_mixer, false);
                _paused = false;
            }
            else
            {
                Bass.ChannelPause(_mixer);
                _paused = true;
            }
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            Bass.ChannelStop(_mixer);
            _paused = false;
        }

        /// <summary>
        /// IsPaused state
        /// </summary>
        public bool IsPaused
        {
            get { return _paused; }
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


        /// <summary>
        /// Change output device
        /// </summary>
        /// <param name="name">string device</param>
        public void ChangeDevice(string name = null)
        {
            if (name == null)
            {
                _initialized = Bass.Init(1, 48000, DeviceInitFlags.Frequency, IntPtr.Zero);
                if (!_initialized)
                {
                    Error("Bass.dll init failed");
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
                    
                    _initialized = Bass.Init(i, 48000, DeviceInitFlags.Frequency, IntPtr.Zero);
                    if (!_initialized)
                    {
                        Error("Bass.dll init failed");
                        return;
                    }
                    Bass.Start();
                }
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
                for (int i = 0; i < BassCd.GetTracks(driveindex); i++)
                {
                    var entry = string.Format("cd://{0}/{1}", driveindex, i);
                    list.Add(entry);
                }
            }
            BassCd.Release(driveindex);
            return list.ToArray();
        }

    }
}
