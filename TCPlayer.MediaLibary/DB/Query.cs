using System;

namespace TCPlayer.MediaLibary.DB
{
    public class QueryInput
    {
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public uint? Year { get; set; }
        public string Geneire { get; set; }

        public string Name { get; set; }

        public QueryInput()
        {
            AlbumName = null;
            Artist = null;
            Year = null;
            Geneire = null;
            Name = null;
        }


        public static QueryInput YearQuery(uint year)
        {
            return new QueryInput
            {
                AlbumName = null,
                Artist = null,
                Geneire = null,
                Year = year
            };
        }

        public static QueryInput ArtistQuery(string artist)
        {
            return new QueryInput
            {
                AlbumName = null,
                Artist = artist,
                Geneire = null,
                Year = null
            };
        }

        public static QueryInput AlbumQuery(string album)
        {
            return new QueryInput
            {
                AlbumName = album,
                Artist = null,
                Geneire = null,
                Year = null
            };
        }

        public static QueryInput GenerireQuery(string geneire)
        {
            return new QueryInput
            {
                AlbumName = null,
                Artist = null,
                Geneire = geneire,
                Year = null
            };
        }

    }
}
