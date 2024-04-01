using System.Timers;

using ManagedBass.Wasapi;

namespace TcPlayer.Engine;

public abstract class EngineBase : IDisposable
{
    protected readonly IMediator _mediator;
    private readonly System.Timers.Timer _timer;
    private bool _disposed;
    private int _counter;

    protected EngineBase(IMediator mediator)
    {
        _counter = 0;
        _mediator = mediator;
        //_timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(200));
        //_timer.Elapsed += OnTimerElapsed;
        //_timer.Start();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) 
            return;

        if (disposing)
        {
            _timer.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private const int _200Miliseconds = 200;
    private const int _5Seconds = 5000;
    private const int _1Seconds = 1000;

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _counter += _200Miliseconds;
        if (_counter >= _5Seconds)
        {
            if (!OnEvery5000Ms()) return;
        }
        if (_counter >= _1Seconds)
        {
            if (!OnEvery1000Ms()) return;
        }
        OnEvery200Ms();
    }

    protected virtual bool OnEvery200Ms() => false;
    protected virtual bool OnEvery1000Ms() => false;
    protected virtual bool OnEvery5000Ms() => false;

    public IEnumerable<DeviceInfo> GetDevices()
    {
        for (int i=0; BassWasapi.GetDeviceInfo(i, out WasapiDeviceInfo info); i++)
        {
            if (info.IsEnabled && !info.IsInput)
            {
                yield return new DeviceInfo
                {
                    Index = i,
                    Name = info.Name,
                    Channels = info.MixChannels,
                    Frequency = info.MixFrequency,
                    UpdatePeriod = info.MinimumUpdatePeriod,
                };
            }
        }
    }
}
