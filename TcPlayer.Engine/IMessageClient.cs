namespace TcPlayer.Engine;

public interface IMessageClient<in T> where T : class
{
    public void OnNotify(T message);
}
