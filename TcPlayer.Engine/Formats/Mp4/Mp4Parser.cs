using System.Text;

namespace TcPlayer.Engine.Formats.Mp4;
internal sealed class Mp4Parser : IDisposable
{
    private Stream? _stream;

    public Mp4Parser(Stream stream)
    {
        _stream = stream;
    }

    public void Dispose()
    {
        if (_stream != null)
        {
            _stream?.Dispose();
            _stream = null;
        }
    }

    public IEnumerable<Chapter> ExtractChapters()
    {
        if (IsMp4a())
        {
            _stream?.Seek(0, SeekOrigin.Begin);
            var moovBox = ReadMoovInfo();
            if (moovBox != null)
            {
                return ReadChapters(moovBox.Value);
            }
        }
        return Enumerable.Empty<Chapter>();

    }

    private IEnumerable<Chapter> ReadChapters(MoovInfo moovBox)
    {
        var soundBox = moovBox.Tracks.Where(b => b.Type == "soun").ToArray();
        if (soundBox.Length == 0) return Enumerable.Empty<Chapter>();
        if (soundBox[0].Chaps != null && soundBox[0].Chaps.Length > 0)
        {
            var cb = new HashSet<uint>(soundBox[0].Chaps);
            var textBox = moovBox.Tracks.Where(b => b.Type == "text" && cb.Contains(b.Id)).ToArray();
            if (textBox.Length == 0) return Enumerable.Empty<Chapter>();
            return ReadChaptersText(textBox[0]);
        }
        return Enumerable.Empty<Chapter>();
    }

    private IEnumerable<Chapter> ReadChaptersText(TrakInfo textBox)
    {
        if (textBox.Durations != null && textBox.Samples != null)
        {
            var len = Math.Min(textBox.Durations.Length, textBox.Samples.Length);
            if (len > 0)
            {
                var pos = 0.0d;
                var tps = (double)textBox.TimeUnitPerSecond;
                if (tps <= 0.1) tps = 600;
                for (int i = 0; i < len; i++)
                {
                    var ChPos = pos;
                    var d = (double)textBox.Durations[i];
                    pos += (d / tps);
                    _stream?.Seek(textBox.Samples[i], SeekOrigin.Begin);

                    yield return new Chapter
                    {
                        Title = ReadPascalString(Encoding.UTF8),
                        StartTimeSeconds = ChPos
                    };
                }
            }
        }
    }

    private bool IsMp4a()
    {
        if (_stream?.Length < 8)
        {
            return false;
        }
        _stream?.Seek(4, SeekOrigin.Begin);
        var t = ReadType();
        return t.Check(Constants.Ftyp);
    }

    private BoxInfo? FindBox(byte[] type)
    {
        BoxInfo? box;
        do
        {
            box = NextBox();
            if (box != null)
            {
                if (box.Value.Type.Check(type))
                {
                    return box;
                }
                SeekNext(box.Value);
            }
        } while (box?.Last == false);
        return null;
    }

    private MoovInfo? ReadMoovInfo()
    {
        var moovBox = FindBox(Constants.Moov);
        if (moovBox != null)
        {
            var moovData = new MoovInfo();
            var tracks = new List<TrakInfo>();
            var maxLen = moovBox.Value.BoxOffset + moovBox.Value.Offset;
            BoxInfo? box;
            do
            {
                box = NextBox(maxLen);
                if (box != null)
                {
                    if (box.Value.Type.Check(Constants.Mvhd))
                    {
                        ReadMvhd(ref moovData);
                    }
                    if (box.Value.Type.Check(Constants.Trak))
                    {
                        tracks.Add(ReadTrak(box.Value));
                    }
                    SeekNext(box.Value);
                }
            }
            while (box?.Last == false);
            moovData.Tracks = tracks.ToArray();
            return moovData;
        }
        return null;
    }

    private TrakInfo ReadTrak(BoxInfo trakBox)
    {
        var maxLen = trakBox.BoxOffset + trakBox.Offset;
        var trakData = new TrakInfo();
        BoxInfo? box;
        do
        {
            box = NextBox(maxLen);
            if (box != null)
            {
                if (box.Value.Type.Check(Constants.Tkhd))
                {
                    ReadTkhd(ref trakData);
                }
                if (box.Value.Type.Check(Constants.Mdia))
                {
                    ReadMdia(ref trakData, box.Value);
                }
                if (box.Value.Type.Check(Constants.Tref))
                {
                    ReadTref(ref trakData, box.Value);
                }
                SeekNext(box.Value);
            }
        } while (box?.Last == false);
        return trakData;
    }

    private void ReadTref(ref TrakInfo trakData, BoxInfo box2)
    {
        var maxLen = box2.BoxOffset + box2.Offset;
        BoxInfo? box;
        do
        {
            box = NextBox(maxLen);
            if (box != null)
            {
                if (box.Value.Type.Check(Constants.Chap))
                {
                    ReadChap(ref trakData, box.Value);
                }
                SeekNext(box.Value);
            }
        } while (box?.Last == false);
    }

    private void ReadChap(ref TrakInfo trakData, BoxInfo box2)
    {
        var len = (box2.Offset - 8) / 4;
        if (len > 0 && len < 1024)
        {
            trakData.Chaps = new uint[len];
            for (uint i = 0; i < len; i++)
            {
                trakData.Chaps[i] = ReadUint32();
            }
        }
    }

    private void ReadMdia(ref TrakInfo trackData, BoxInfo mdiaBox)
    {
        var maxLen = mdiaBox.BoxOffset + mdiaBox.Offset;
        BoxInfo? box;
        do
        {
            box = NextBox(maxLen);
            if (box != null)
            {
                if (box.Value.Type.Check(Constants.Mdhd))
                {
                    ReadMdhd(ref trackData);
                }
                if (box.Value.Type.Check(Constants.Hdlr))
                {
                    ReadHdlr(ref trackData);
                }
                if (box.Value.Type.Check(Constants.Minf))
                {
                    ReadMinf(ref trackData, box.Value);
                }
                SeekNext(box.Value);
            }
        } while (box?.Last == false);
    }

    private void ReadMinf(ref TrakInfo trakData, BoxInfo box2)
    {
        var maxLen = box2.BoxOffset + box2.Offset;
        BoxInfo? box;
        do
        {
            box = NextBox(maxLen);
            if (box != null)
            {
                if (box.Value.Type.Check(Constants.Stbl))
                {
                    ReadStbl(ref trakData, box.Value);
                }
                SeekNext(box.Value);
            }
        } while (box?.Last == false);
    }

    private void ReadStbl(ref TrakInfo trakData, BoxInfo box2)
    {
        var maxLen = box2.BoxOffset + box2.Offset;
        BoxInfo? box;
        do
        {
            box = NextBox(maxLen);
            if (box != null)
            {
                if (box.Value.Type.Check(Constants.Stco))
                {
                    ReadStco(ref trakData);
                }
                if (box.Value.Type.Check(Constants.Stts))
                {
                    ReadStts(ref trakData);
                }
                SeekNext(box.Value);
            }
        } while (box?.Last == false);
    }

    private void ReadStts(ref TrakInfo trakData)
    {
        _stream?.Seek(4, SeekOrigin.Current);
        var len = ReadUint32();
        if (len > 1024) len = 0;
        trakData.Durations = new uint[len];
        trakData.FrameCount = new uint[len];
        for (uint i = 0; i < len; i++)
        {
            trakData.FrameCount[i] = ReadUint32();
            trakData.Durations[i] = ReadUint32();
        }
    }

    private void ReadStco(ref TrakInfo trakData)
    {
        _stream?.Seek(4, SeekOrigin.Current);
        var len = ReadUint32();
        if (len > 1024) len = 0;
        trakData.Samples = new long[len];
        for (uint i = 0; i < len; i++)
        {
            trakData.Samples[i] = ReadUint32();
        }
    }

    private void ReadHdlr(ref TrakInfo trakData)
    {
        _stream?.Seek(4 + 4, SeekOrigin.Current);
        var b = new byte[4];
        _stream?.Read(b, 0, 4);
        var bc = new char[4];
        Encoding.ASCII.GetDecoder().GetChars(b, 0, 4, bc, 0);
        trakData.Type = new string(bc);
    }

    private void ReadMdhd(ref TrakInfo trakData)
    {
        var v = new byte[1];
        _stream?.Read(v, 0, 1);
        var isv8 = v[0] == 1;
        _stream?.Seek(3 + (isv8 ? 8 + 8 : 4 + 4), SeekOrigin.Current);
        trakData.TimeUnitPerSecond = ReadUint32();
    }

    private void ReadTkhd(ref TrakInfo trakData)
    {
        var v = new byte[1];
        _stream?.Read(v, 0, 1);
        var isv8 = v[0] == 1;
        _stream?.Seek(3 + (isv8 ? 8 + 8 : 4 + 4), SeekOrigin.Current);
        trakData.Id = ReadUint32();
    }

    private void ReadMvhd(ref MoovInfo moovData)
    {
        var v = new byte[1];
        _stream?.Read(v, 0, 1);
        var isv8 = v[0] == 1;
        _stream?.Seek(3 + (isv8 ? 8 + 8 : 4 + 4), SeekOrigin.Current);
        moovData.TimeUnitPerSecond = ReadUint32();
    }

    private void SeekNext(BoxInfo box)
    {
        _stream?.Seek(box.BoxOffset, SeekOrigin.Begin);
        _stream?.Seek(box.Offset, SeekOrigin.Current);
    }

    private BoxInfo? NextBox(long? maxLen = null)
    {
        var cp = _stream?.Position ?? 0;
        if ((maxLen ?? _stream?.Length) - _stream?.Position < 8) return null;
        long ofs = ReadUint32();
        var at = ReadType();
        if (!at.Check(Constants.Mdat))
        {
            return new BoxInfo()
            {
                BoxOffset = cp,
                Offset = ofs,
                Last = ofs == 0,
                Type = at
            };
        }
        if ((maxLen ?? _stream?.Length) - _stream?.Position < 8) return null;
        if (ofs == 1)
        {
            ofs = (long)ReadUint64();
        }
        else
        {
            _stream?.Seek(8, SeekOrigin.Current);
        }
        return new BoxInfo()
        {
            BoxOffset = cp,
            Offset = ofs,
            Last = ofs == 0,
            Type = at
        };
    }

    private AsciiType ReadType()
    {
        var b = new byte[4];
        _stream?.Read(b, 0, 4);
        return new AsciiType(b);
    }

    private ushort ReadUint16()
    {
        var b = new byte[2];
        var sz = _stream?.Read(b, 0, 2);
        if (sz != 2) return 0;
        if (BitConverter.IsLittleEndian)
        {
            var b2 = new byte[2];
            b2[0] = b[1];
            b2[1] = b[0];
            return BitConverter.ToUInt16(b2, 0);
        }
        return BitConverter.ToUInt16(b, 0);
    }

    private uint ReadUint32()
    {
        var b = new byte[4];
        var sz = _stream?.Read(b, 0, 4);
        if (sz != 4) return 0;
        if (BitConverter.IsLittleEndian)
        {
            var b2 = new byte[4];
            b2[0] = b[3];
            b2[1] = b[2];
            b2[2] = b[1];
            b2[3] = b[0];
            return BitConverter.ToUInt32(b2, 0);
        }
        return BitConverter.ToUInt32(b, 0);
    }

    private ulong ReadUint64()
    {
        var b = new byte[8];
        var sz = _stream?.Read(b, 0, 8);
        if (sz != 8) return 0;
        if (BitConverter.IsLittleEndian)
        {
            var b2 = new byte[8];
            b2[0] = b[7];
            b2[1] = b[6];
            b2[2] = b[5];
            b2[3] = b[4];
            b2[4] = b[3];
            b2[5] = b[2];
            b2[6] = b[1];
            b2[7] = b[0];
            return BitConverter.ToUInt64(b2, 0);
        }
        return BitConverter.ToUInt64(b, 0);
    }

    private string ReadPascalString(Encoding encoding)
    {
        var sz = ReadUint16();
        if (sz == 0) return "";
        var b = new byte[sz];
        _stream?.Read(b, 0, sz);
        return new string(encoding.GetChars(b));
    }
}
