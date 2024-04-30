namespace TcPlayer.Engine.Notifications;

internal sealed record class EngineLoadNotification
{
    public required MetaData MetaData { get; init; }
}
