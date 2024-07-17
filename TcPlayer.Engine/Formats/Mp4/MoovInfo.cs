namespace TcPlayer.Engine.Formats.Mp4;

internal struct MoovInfo
{
    public uint TimeUnitPerSecond { get; set; }
    public TrakInfo[] Tracks { get; set; }
}
