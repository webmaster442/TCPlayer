using System.Text;

namespace TcPlayer.Engine.Formats.Mp4;

internal readonly struct AsciiType
{
    private readonly byte _type0;
    private readonly byte _type1;
    private readonly byte _type2;
    private readonly byte _type3;

    public AsciiType(byte[] type)
    {
        _type0 = type[0];
        _type1 = type[1];
        _type2 = type[2];
        _type3 = type[3];
    }

    public readonly bool Check(byte[] refType)
    {
        return _type0 == refType[0] &&
               _type1 == refType[1] &&
               _type2 == refType[2] &&
               _type3 == refType[3];
    }

    public override readonly string ToString()
    {
        var enc = Encoding.ASCII;
        var c = new char[4];
        enc.GetDecoder().GetChars([_type0, _type1, _type2, _type3], 0, 4, c, 0);
        return new string(c);
    }
}
