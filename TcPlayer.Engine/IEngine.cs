namespace TcPlayer.Engine;

public interface IEngine
{
    void Init(DeviceInfo info);
    void Load(EngineFile file);
    void Pause();
    void Play();
    void SetPosition(double position);
    void SetVolume(float volume);
    void Stop();
    IEnumerable<DeviceInfo> GetDevices();
}