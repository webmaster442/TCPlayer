using System.Diagnostics;

using ManagedBass;
using ManagedBass.Mix;
using ManagedBass.Wasapi;

namespace TcPlayer.Engine;

public sealed class Engine : EngineBase, IEngine
{
    private DeviceInfo? _info;
    private float _volume;
    private int _decodeChannel;
    private int _mixerChanel;
    private readonly WasapiProcedure _onWasapiUpdateDelegate;
    private readonly string[] _plugins;
    private readonly int[] _loadedPluginHandles;

    public MetaData MetaData { get; private set; }

    public Engine(IMediator mediator) : base(mediator)
    {
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
        for (int i=0; i< _plugins.Length; i++)
        {
            _loadedPluginHandles[i] = Bass.PluginLoad(_plugins[i]);
            if (_loadedPluginHandles[i] == 0)
                throw new EngineException($"Plugin load failed: {Bass.LastError}");
        }
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

    private void DisposeDevice()
    {
        if (_info != null)
        {
            BassWasapi.Free();
            Bass.Free();
            _info = null;
        }
    }

    private int OnWasapiUpdate(nint Buffer, int Length, nint User)
    {
        return Bass.ChannelGetData(_mixerChanel, Buffer, Length);
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
            _volume = BassWasapi.GetVolume(WasapiVolumeTypes.Session | WasapiVolumeTypes.WindowsHybridCurve);
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
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private void SetupMixer()
    {
        if (_info == null)
            throw new EngineException("No output device has been initialized");

        if (_decodeChannel == 0)
            throw new EngineException($"Decode channel create failed: {Bass.LastError}");

        _mixerChanel = BassMix.CreateMixerStream(_info.Frequency,
                                                 _info.Channels,
                                                 BassFlags.Float | BassFlags.Decode | BassFlags.MixerPositionEx);

        if (_mixerChanel == 0)
            throw new EngineException($"Mixer channel create failed: {Bass.LastError}");

        if (!BassMix.MixerAddChannel(_mixerChanel, _decodeChannel, BassFlags.MixerChanDownMix))
        {
            throw new EngineException($"Mixer setup failed");
        }
    }

    public void Play()
    {
        Bass.ChannelPlay(_mixerChanel);
    }

    public void Stop()
    {
        Bass.ChannelStop(_mixerChanel);
        Bass.ChannelStop(_decodeChannel);
        _mixerChanel = 0;
        _decodeChannel = 0;
    }
}
