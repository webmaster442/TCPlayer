namespace TcPlayer.Engine.Notifications;

public sealed record class EngineLoadNotification
{
    public required MetaData MetaData { get; init; }
}
