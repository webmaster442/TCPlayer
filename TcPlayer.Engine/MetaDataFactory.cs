namespace TcPlayer.Engine
{
    internal static class MetaDataFactory
    {
        public static MetaData CreateFromFileName(string filename)
        {
            var file = TagLib.File.Create(filename);

            return new()
            {
                {
                    file.Tag.FirstPerformer,
                    file.Tag.Title,
                    file.Tag.Album,
                    file.Tag.Year.ToString()
                },
                
            };
        }

        internal static MetaData CreateEmpty()
        {
            return new MetaData();
        }
    }
}
