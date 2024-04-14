namespace TcPlayer.Engine;

internal sealed record class EngineLoadNotification
{
    public required MetaData MetaData { get; init; }
}
