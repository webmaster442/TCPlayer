namespace TcPlayer.Engine.Internals
{
    internal static class MetaDataFactory
    {
        public static MetaData CreateFromFileName(string filename)
        {
            var file = TagLib.File.Create(filename);

            TryGetCover(file, out byte[] data, out string mime);

            return new()
            {
                Cover = data,
                CoverMime = mime,
                Data = new List<string>
                {
                    GetOrDefault(file.Tag.FirstPerformer, "Unknown Artist"),
                    GetOrDefault(file.Tag.Title, "Unknown song"),
                    GetOrDefault(file.Tag.Album, "Unknown album"),
                    file.Tag.Year.ToString()
                }
            };
        }

        private static string GetOrDefault(string value, string defaultValue) 
            => string.IsNullOrEmpty(value) ? defaultValue : value;

        private static bool TryGetCover(TagLib.File file, out byte[] data, out string mime)
        {
            if (file.Tag.Pictures.Length == 0)
            {
                data = Array.Empty<byte>();
                mime = string.Empty;
                return false;
            }

            var picture = file.Tag.Pictures
                .FirstOrDefault(p => p.Type == TagLib.PictureType.FrontCover, file.Tag.Pictures[0]);

            data = picture.Data.Data;
            mime = picture.MimeType;
            return true;
        }

        internal static MetaData CreateEmpty()
        {
            return new MetaData();
        }
    }
}
