using ManagedBass;

namespace TcPlayer.Engine;

[Serializable]
public class EngineException : Exception
{
    public EngineException() : base($"Bass error: {Bass.LastError}")
    {
    }

    public EngineException(string? message) : base(message)
    {
    }

    public EngineException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
