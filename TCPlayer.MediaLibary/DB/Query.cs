using System;

namespace TCPlayer.MediaLibary.DB
{
    public class QueryInput
    {
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public uint? Year { get; set; }

        public QueryInput()
        {
            AlbumName = null;
            Artist = null;
            Year = null;
        }

        public QueryInput(string album, string artist, uint? year)
        {
            AlbumName = album;
            Artist = artist;
            Year = year;
        }
    }

    public class YearQuery : QueryInput
    {
        public YearQuery(uint year):
            base(null, null, year)
        {
        }
    }
}
