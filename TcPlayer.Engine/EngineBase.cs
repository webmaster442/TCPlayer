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
        _timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(100));
        _timer.Elapsed += OnTimerElapsed;
    }

    protected void TimerStart() 
        => _timer.Start();

    protected void TimerStop()
        => _timer.Stop();

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

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _counter++;
        OnEvery100ms();
        if (_counter > 9)
        {
            OnEvery1000ms();
            _counter = 0;
        }
    }

    protected virtual void OnEvery1000ms()
    {
        //empty in base class
    }

    protected virtual void OnEvery100ms()
    {
        //empty in base class
    }

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
