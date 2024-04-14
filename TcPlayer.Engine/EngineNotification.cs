namespace TcPlayer.Engine;

internal sealed record class EngineNotification
{
    public required double Length { get; init; }
    public required double Position { get; init; }
    public required float Volume { get; init; }
    public required EngineState EngineState { get; init; }
}
