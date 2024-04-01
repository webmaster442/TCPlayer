namespace TcPlayer.Engine;

public class Mediator : IMediator
{
    private readonly List<WeakReference> _clients;

    public Mediator()
    {
        _clients = new List<WeakReference>();
    }

    public void Register(object client)
    {
        _clients.Add(new WeakReference(client));
    }

    public void Notify<T>(T message) where T : class
    {
        Stack<WeakReference> dead = new();
        foreach (var client in _clients) 
        {
            if (!client.IsAlive)
            {
                dead.Push(client);
                continue;
            }
            
            if (client.Target is IMessageClient<T> messageClient) 
            {
                messageClient.OnNotify(message);
            }
        }
        CleanupDead(dead);
    }

    private void CleanupDead(Stack<WeakReference> dead)
    {
        WeakReference toRemove;
        while (dead.Count > 0)
        {
            toRemove = dead.Pop();
            _clients.Remove(toRemove);
        }
    }
}
