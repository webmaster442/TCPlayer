namespace Mp4Chapters
{
    internal struct MoovInfo
    {
        public uint TimeUnitPerSecond { get; set; }

        public TrakInfo[] Tracks { get; set; }
    }

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
}