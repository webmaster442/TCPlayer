namespace TcPlayer.Engine;

public interface IMediator
{
    public void Notify<T>(T message) where T : class;
    public void Register(object client);
}
