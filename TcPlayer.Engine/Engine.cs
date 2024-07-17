using System.Diagnostics;

using ManagedBass;
using ManagedBass.Mix;
using ManagedBass.Wasapi;

using TcPlayer.Engine.Internals;
using TcPlayer.Engine.Notifications;

namespace TcPlayer.Engine;

public sealed class Engine : EngineBase, IEngine
{
    private DeviceInfo? _info;
    private int _decodeChannel;
    private int _mixerChanel;
    private bool _notificationBlock;
    private readonly WasapiProcedure _onWasapiUpdateDelegate;
    private readonly string[] _plugins;
    private readonly int[] _loadedPluginHandles;
    private readonly IPlaylist _playlist;

    public MetaData MetaData { get; private set; }

    public Engine(IMediator mediator, IPlaylist playlist) : base(mediator)
    {
        _notificationBlock = false;
        _plugins =
            [
                Path.Combine(AppContext.BaseDirectory, "bass_aac.dll"),
                Path.Combine(AppContext.BaseDirectory, "bass_ac3.dll"),
                Path.Combine(AppContext.BaseDirectory, "bassalac.dll"),
                Path.Combine(AppContext.BaseDirectory, "bassape.dll"),
                Path.Combine(AppContext.BaseDirectory, "bassflac.dll"),
                Path.Combine(AppContext.BaseDirectory, "basswma.dll"),
                Path.Combine(AppContext.BaseDirectory, "basswv.dll"),

            ];
        MetaData = MetaDataFactory.CreateEmpty();
        _onWasapiUpdateDelegate = new WasapiProcedure(OnWasapiUpdate);
        _loadedPluginHandles = new int[_plugins.Length];
        for (int i = 0; i < _plugins.Length; i++)
        {
            _loadedPluginHandles[i] = Bass.PluginLoad(_plugins[i]);
            if (_loadedPluginHandles[i] == 0)
                throw new EngineException($"Plugin load failed: {Bass.LastError}");
        }
        _playlist = playlist;
    }

    ~Engine()
    {
        Dispose(false);
    }

    protected override void Dispose(bool disposing)
    {
        DisposeDevice();
        foreach (var pluginHandle in _loadedPluginHandles)
        {
            Bass.PluginFree(pluginHandle);
        }
        base.Dispose(disposing);
    }

    protected override void OnEvery100ms()
    {
        if (_notificationBlock)
            return;

        UpdatePosition();

        if (Length > 0
            && (Length - Position) < 0.2
            && _playlist.CurrentIndex + 1 < _playlist.Count)
        {
            _playlist.CurrentIndex += 1;
            Load(_playlist[_playlist.CurrentIndex]);
        }
    }

    private void DisposeDevice()
    {
        if (_info != null)
        {
            BassWasapi.Free();
            Bass.Free();
            _info = null;
        }
    }

    private int OnWasapiUpdate(nint buffer, int length, nint user) 
        => Bass.ChannelGetData(_mixerChanel, buffer, length);

    private void SetupMixer()
    {
        if (_info == null)
            throw new EngineException("No output device has been initialized");

        if (_decodeChannel == 0)
            throw new EngineException($"Decode channel create failed: {Bass.LastError}");

        if (_mixerChanel == 0)
        {
            _mixerChanel = BassMix.CreateMixerStream(_info.Frequency,
                                                     _info.Channels,
                                                     BassFlags.Float | BassFlags.Decode | BassFlags.MixerPositionEx);

            if (_mixerChanel == 0)
                throw new EngineException($"Mixer channel create failed: {Bass.LastError}");
        }

        if (!BassMix.MixerAddChannel(_mixerChanel, _decodeChannel, BassFlags.MixerChanDownMix))
        {
            throw new EngineException($"Mixer setup failed");
        }
    }

    private void SetupInitialLengthAndPosition()
    {
        long len = Bass.ChannelGetLength(_decodeChannel, PositionFlags.Bytes);
        if (len < 0)
            Length = double.PositiveInfinity;
        else
            Length = Bass.ChannelBytes2Seconds(_decodeChannel, len);

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        long pos = BassMix.ChannelGetPosition(_decodeChannel, PositionFlags.Bytes);
        Position = Bass.ChannelBytes2Seconds(_decodeChannel, pos);
        SendNotification();
    }

    private void SendNotification()
    {
        _mediator.Notify(new EngineStateChangeNotification
        {
            LengthSeconds = this.Length,
            PositionSeconds = this.Position,
            EngineState = this.State,
            Volume = this.Volume,
        });
    }

    public void Init(DeviceInfo info)
    {
        DisposeDevice();
        if (Bass.Init(0, info.Frequency, DeviceInitFlags.Default)
            && BassWasapi.Init(info.Index,
                               info.Frequency,
                               info.Channels,
                               WasapiInitFlags.Buffer | WasapiInitFlags.AutoFormat,
                               (float)(info.UpdatePeriod * 2),
                               (float)info.UpdatePeriod,
                               _onWasapiUpdateDelegate)
            && BassWasapi.Start())
        {
            _info = info;
            Volume = BassWasapi.GetVolume(WasapiVolumeTypes.Session | WasapiVolumeTypes.WindowsHybridCurve);
        }
        else
        {
            throw new EngineException();
        }
    }

    public void Load(EngineFile file)
    {
        MetaData = MetaDataFactory.CreateEmpty();
        Stop();

        if (file.TryGetFileName(out string fileName))
        {
            MetaData = MetaDataFactory.CreateFromFileName(fileName);
            _decodeChannel = Bass.CreateStream(fileName,
                                               Offset: 0,
                                               Length: 0,
                                               BassFlags.Decode | BassFlags.Float);
            SetupMixer();
            SetupInitialLengthAndPosition();
            _mediator.Notify(new EngineLoadNotification
            {
                MetaData = MetaData
            });
        }
        else
        {
            throw new UnreachableException();
        }
    }

    public void Pause()
    {
        Bass.ChannelStop(_mixerChanel);
        State = EngineState.Pause;
        SendNotification();
        TimerStop();
    }

    public void Play()
    {
        Bass.ChannelSetPosition(_mixerChanel, 0, PositionFlags.Bytes);
        State = EngineState.Play;
        SendNotification();
        TimerStart();
    }

    public void Stop()
    {
        Bass.ChannelStop(_mixerChanel);
        Bass.ChannelStop(_decodeChannel);
        BassMix.MixerRemoveChannel(_decodeChannel);
        Bass.StreamFree(_decodeChannel);
        _decodeChannel = 0;
        State = EngineState.Stop;
        SendNotification();
        TimerStop();
    }

    public void SetVolume(float volume)
    {
        _notificationBlock = true;
        if (_info == null)
            throw new EngineException("Engine is not initialized");
        float value = volume > 1.0f ? 1.0f : volume;
        value = value < 0.0f ? 0.0f : value;
        BassWasapi.SetVolume(WasapiVolumeTypes.Session | WasapiVolumeTypes.WindowsHybridCurve, value);
        _notificationBlock = false;
    }

    public void SetPosition(double position)
    {
        _notificationBlock = true;
        if (_info == null)
            throw new EngineException("Engine is not initialized");

        if (double.IsInfinity(Length))
            return;

        var bytes = Bass.ChannelSeconds2Bytes(_decodeChannel, position);
        Bass.ChannelSetPosition(_decodeChannel, bytes);
        _notificationBlock = false;
    }

    public double Position { get; private set; }

    public double Length { get; private set; }

    public EngineState State { get; private set; }

    public float Volume { get; private set; }
}
