namespace TcPlayer.Engine;

public sealed record class Chapter
{
    public required string Title { get; init; }
    public required double StartTimeSeconds { get; init; }
}
