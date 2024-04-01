namespace TcPlayer.Engine;

public record class DeviceInfo
{
    public required string Name { get; init; }
    public required int Index { get; init; }
    public required int Channels { get; init; }
    public required int Frequency { get; init; }
    public required double UpdatePeriod { get; init; }
}
