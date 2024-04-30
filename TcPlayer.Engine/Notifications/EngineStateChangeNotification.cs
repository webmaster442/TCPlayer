namespace TcPlayer.Engine.Notifications;

public sealed record class EngineStateChangeNotification
{
    public required double LengthSeconds { get; init; }
    public required double PositionSeconds { get; init; }
    public required float Volume { get; init; }
    public required EngineState EngineState { get; init; }
}
