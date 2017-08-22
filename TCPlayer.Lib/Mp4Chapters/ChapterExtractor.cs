using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading;

namespace Mp4Chapters
{
    /// <summary>
    /// Средство извлечения информации о треках.
    /// </summary>
    public sealed class ChapterExtractor
    {
        private static readonly byte[] tref = new byte[] {0x74, 0x72, 0x65, 0x66};

        private static readonly byte[] chap = new byte[] {0x63, 0x68, 0x61, 0x70};

        private static readonly byte[] mdat = new byte[] {0x6d, 0x64, 0x61, 0x74};

        private static readonly byte[] moov = new byte[] {0x6d, 0x6f, 0x6f, 0x76};

        private static readonly byte[] mvhd = new byte[] {0x6d, 0x76, 0x68, 0x64};

        private static readonly byte[] trak = new byte[] {0x74, 0x72, 0x61, 0x6b};

        private static readonly byte[] tkhd = new byte[] {0x74, 0x6b, 0x68, 0x64};

        private static readonly byte[] mdia = new byte[] {0x6d, 0x64, 0x69, 0x61};

        private static readonly byte[] mdhd = new byte[] {0x6d, 0x64, 0x68, 0x64};

        private static readonly byte[] hdlr = new byte[] {0x68, 0x64, 0x6c, 0x72};

        private static readonly byte[] minf = new byte[] {0x6d, 0x69, 0x6e, 0x66};

        private static readonly byte[] stbl = new byte[] {0x73, 0x74, 0x62, 0x6c};

        private static readonly byte[] stco = new byte[] {0x73, 0x74, 0x63, 0x6f};

        private static readonly byte[] stts = new byte[] {0x73, 0x74, 0x74, 0x73};

        private static readonly byte[] ftyp = new byte[] {0x66, 0x74, 0x79, 0x70};

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="stream">Поток.</param>
        public ChapterExtractor(IAbstractStream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Главы.
        /// </summary>
        public ChapterInfo[] Chapters { get; set; }

        private readonly IAbstractStream stream;

        /// <summary>
        /// Анализировать.
        /// </summary>
        public void Run()
        {
            Chapters = null;
            stream.Seek(SeekOrigin.Begin, 0);
            var moovBox = ReadMoovInfo();
            if (moovBox != null)
            {
                ReadChapters(moovBox.Value);
            }
        }

        private void ReadChapters(MoovInfo moovBox)
        {
            var soundBox = moovBox.Tracks.Where(b => b.Type == "soun").ToArray();
            if (soundBox.Length == 0) return;
            if (soundBox[0].Chaps != null && soundBox[0].Chaps.Length > 0)
            {
                var cb = new HashSet<uint>(soundBox[0].Chaps);
                var textBox = moovBox.Tracks.Where(b => b.Type == "text" && cb.Contains(b.Id)).ToArray();
                if (textBox.Length == 0) return;
                ReadChaptersText(textBox[0]);
            }
        }

        private void ReadChaptersText(TrakInfo textBox)
        {
            if (textBox.Durations != null && textBox.Samples != null)
            {
                var len = Math.Min(textBox.Durations.Length, textBox.Samples.Length);
                if (len > 0)
                {
                    Chapters = new ChapterInfo[len];
                    var pos = TimeSpan.FromSeconds(0);
                    var tps = (double) textBox.TimeUnitPerSecond;
                    if (tps <= 0.1) tps = 600;
                    for (int i = 0; i < len; i++)
                    {
                        var ci = new ChapterInfo();
                        ci.Time = pos;
                        var d = (double) textBox.Durations[i];
                        pos += TimeSpan.FromSeconds(d / tps);
                        stream.Seek(SeekOrigin.Begin, textBox.Samples[i]);
                        ci.Name = ReadPascalString(Encoding.UTF8);
                        Chapters[i] = ci;
                    }
                }
            }
        }

        /// <summary>
        /// Является mp4a.
        /// </summary>
        /// <returns>Результат.</returns>
        public bool IsMp4a()
        {
            if (stream.Length < 8)
            {
                return false;
            }
            stream.Seek(SeekOrigin.Begin, 4);
            var t = ReadType();
            return t.Check(ftyp);
        }


        private BoxInfo? FindBox(byte[] type)
        {
            BoxInfo? box = null;
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
            } while (!(box == null || box.Value.Last));
            return null;
        }

        private MoovInfo? ReadMoovInfo()
        {
            var moovBox = FindBox(moov);
            if (moovBox != null)
            {
                var moovData = new MoovInfo();
                var tracks = new List<TrakInfo>();
                var maxLen = moovBox.Value.BoxOffset + moovBox.Value.Offset;
                BoxInfo? box = null;
                do
                {
                    box = NextBox(maxLen);
                    if (box != null)
                    {
                        if (box.Value.Type.Check(mvhd))
                        {
                            ReadMvhd(ref moovData);
                        }
                        if (box.Value.Type.Check(trak))
                        {
                            tracks.Add(ReadTrak(box.Value));
                        }
                        SeekNext(box.Value);
                    }
                } while (!(box == null || box.Value.Last));
                moovData.Tracks = tracks.ToArray();
                return moovData;
            }
            return null;
        }

        private TrakInfo ReadTrak(BoxInfo trakBox)
        {
            var maxLen = trakBox.BoxOffset + trakBox.Offset;
            var trakData = new TrakInfo();
            BoxInfo? box = null;
            do
            {
                box = NextBox(maxLen);
                if (box != null)
                {
                    if (box.Value.Type.Check(tkhd))
                    {
                        ReadTkhd(ref trakData);
                    }
                    if (box.Value.Type.Check(mdia))
                    {
                        ReadMdia(ref trakData, box.Value);
                    }
                    if (box.Value.Type.Check(tref))
                    {
                        ReadTref(ref trakData, box.Value);
                    }
                    SeekNext(box.Value);
                }
            } while (!(box == null || box.Value.Last));
            return trakData;
        }

        private void ReadTref(ref TrakInfo trakData, BoxInfo box2)
        {
            var maxLen = box2.BoxOffset + box2.Offset;
            BoxInfo? box = null;
            do
            {
                box = NextBox(maxLen);
                if (box != null)
                {
                    if (box.Value.Type.Check(chap))
                    {
                        ReadChap(ref trakData, box.Value);
                    }
                    SeekNext(box.Value);
                }
            } while (!(box == null || box.Value.Last));                        
        }

        private void ReadChap(ref TrakInfo trakData, BoxInfo box2)
        {
            var len = (box2.Offset - 8)/4;
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
            BoxInfo? box = null;
            do
            {
                box = NextBox(maxLen);
                if (box != null)
                {
                    if (box.Value.Type.Check(mdhd))
                    {
                        ReadMdhd(ref trackData);
                    }
                    if (box.Value.Type.Check(hdlr))
                    {
                        ReadHdlr(ref trackData);
                    }
                    if (box.Value.Type.Check(minf))
                    {
                        ReadMinf(ref trackData, box.Value);
                    }
                    SeekNext(box.Value);
                }
            } while (!(box == null || box.Value.Last));
        }

        private void ReadMinf(ref TrakInfo trakData, BoxInfo box2)
        {
            var maxLen = box2.BoxOffset + box2.Offset;
            BoxInfo? box = null;
            do
            {
                box = NextBox(maxLen);
                if (box != null)
                {
                    if (box.Value.Type.Check(stbl))
                    {
                        ReadStbl(ref trakData, box.Value);
                    }
                    SeekNext(box.Value);
                }
            } while (!(box == null || box.Value.Last));            
        }

        private void ReadStbl(ref TrakInfo trakData, BoxInfo box2)
        {
            var maxLen = box2.BoxOffset + box2.Offset;
            BoxInfo? box = null;
            do
            {
                box = NextBox(maxLen);
                if (box != null)
                {
                    if (box.Value.Type.Check(stco))
                    {
                        ReadStco(ref trakData);
                    }
                    if (box.Value.Type.Check(stts))
                    {
                        ReadStts(ref trakData);
                    }
                    SeekNext(box.Value);
                }
            } while (!(box == null || box.Value.Last));
        }

        private void ReadStts(ref TrakInfo trakData)
        {
            stream.Seek(SeekOrigin.Current, 4);
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
            stream.Seek(SeekOrigin.Current, 4);
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
            stream.Seek(SeekOrigin.Current, 4 + 4);
            var b = new byte[4];
            stream.Read(b, 4);
            var bc = new char[4];
            AsciiEncoding.Current.GetDecoder().GetChars(b, 0, 4, bc, 0);
            trakData.Type = new string(bc);
        }

        private void ReadMdhd(ref TrakInfo trakData)
        {
            var v = new byte[1];
            stream.Read(v, 1);
            var isv8 = v[0] == 1;
            stream.Seek(SeekOrigin.Current, 3 + (isv8 ? 8 + 8 : 4 + 4));
            trakData.TimeUnitPerSecond = ReadUint32();
        }

        private void ReadTkhd(ref TrakInfo trakData)
        {
            var v = new byte[1];
            stream.Read(v, 1);
            var isv8 = v[0] == 1;
            stream.Seek(SeekOrigin.Current, 3 + (isv8 ? 8 + 8 : 4 + 4));
            trakData.Id = ReadUint32();
        }

        private void ReadMvhd(ref MoovInfo moovData)
        {
            var v = new byte[1];
            stream.Read(v, 1);
            var isv8 = v[0] == 1;
            stream.Seek(SeekOrigin.Current, 3 + (isv8 ? 8 + 8 : 4 + 4));
            moovData.TimeUnitPerSecond = ReadUint32();
        }

#if DEBUG
        private void DebugBoxes(long? maxLen)
        {
            BoxInfo? box = null;
            do
            {
                box = NextBox(maxLen);
                if (box != null)
                {
                    Debug.WriteLine("{0}: {1} -> {2}", box.Value.Type, box.Value.BoxOffset, box.Value.Offset);
                    SeekNext(box.Value);
                }
            } while (!(box == null || box.Value.Last));                            
        }
#endif

        private void SeekNext(BoxInfo box)
        {
            stream.Seek(SeekOrigin.Begin, box.BoxOffset);
            stream.Seek(SeekOrigin.Current, box.Offset);            
        }

        private BoxInfo? NextBox(long? maxLen = null)
        {
            var cp = stream.Position;
            if ((maxLen ?? stream.Length) - stream.Position < 8) return null;
            long ofs = ReadUint32();
            var at = ReadType();
            if (!at.Check(mdat))
            {
                return new BoxInfo()
                {
                    BoxOffset = cp,
                    Offset = ofs,
                    Last = ofs == 0,
                    Type = at
                };
            }
            if ((maxLen ?? stream.Length) - stream.Position < 8) return null;
            if (ofs == 1)
            {
                ofs = (long)ReadUint64();
            }
            else
            {
                stream.Seek(SeekOrigin.Current, 8);                
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
            stream.Read(b, 4);
            return new AsciiType()
            {
                Type = b
            };
        }

        private ushort ReadUint16()
        {
            var b = new byte[2];
            var sz = stream.Read(b, 2);
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
            var sz = stream.Read(b, 4);
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
            var sz = stream.Read(b, 8);
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
            stream.Read(b, sz);
            return new string(encoding.GetChars(b));
        }
    }
}