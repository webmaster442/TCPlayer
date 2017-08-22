using System.Text;

namespace Mp4Chapters
{
    internal struct BoxInfo
    {
        public long Offset { get; set; }

        public long BoxOffset { get; set; }

        public bool Last { get; set; }

        public AsciiType Type { get; set; }
    }

    internal struct AsciiType
    {
        public byte[] Type { get; set; }

        public bool Check(byte[] refType)
        {
            return Type[0] == refType[0] &&
                   Type[1] == refType[1] &&
                   Type[2] == refType[2] &&
                   Type[3] == refType[3];
        }

        public override string ToString()
        {
            var enc = AsciiEncoding.Current;
            var c = new char[4];
            enc.GetDecoder().GetChars(Type, 0, 4, c, 0);
            return new string(c);
        }
    }
}