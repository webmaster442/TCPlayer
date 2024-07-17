namespace TcPlayer.Engine.Formats.Mp4;

internal struct TrakInfo
{
    public uint Id { get; set; }
    public string Type { get; set; }
    public long[] Samples { get; set; }
    public uint[] Durations { get; set; }
    public uint[] FrameCount { get; set; }
    public uint[] Chaps { get; set; }
    public uint TimeUnitPerSecond { get; set; }
}