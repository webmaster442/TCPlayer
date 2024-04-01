namespace TcPlayer.Engine;

public interface IEngine : IDisposable
{
    void Init(DeviceInfo info);
    public IEnumerable<DeviceInfo> GetDevices();
    void Load(EngineFile file);
    void Stop();
    MetaData MetaData { get; }
    //void Play();
    //void Pause();
    //float Volume { get; set; }
    //double Position { get; set; }
    //double Duration { get; }
}
