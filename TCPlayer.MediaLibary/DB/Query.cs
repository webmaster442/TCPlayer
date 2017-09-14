/*
    TC Plyer
    Total Commander Audio Player plugin & standalone player written in C#, based on bass.dll components
    Copyright (C) 2016 Webmaster442 aka. Ruzsinszki Gábor

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;

namespace TCPlayer.MediaLibary.DB
{
    public enum QueryOperator: int
    {
        Default,
        Equals,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual
    }

    [Serializable]
    public class QueryInput
    {
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public uint? Year { get; set; }
        public string Geneire { get; set; }
        public string Name { get; set; }

        public QueryOperator RatingOperator;
        public short? Rating { get; set; }

        public QueryOperator PlayCounterOperator;
        public uint? PlayCount { get; set; }

        public string AlbumArtist { get; set; }


        public QueryInput()
        {
            AlbumName = null;
            Artist = null;
            Year = null;
            Geneire = null;
            Name = null;
            Rating = null;
            RatingOperator = QueryOperator.Default;
        }


        public static QueryInput YearQuery(uint year)
        {
            return new QueryInput
            {
                AlbumName = null,
                Artist = null,
                Geneire = null,
                Year = year,
                Rating = null,
                RatingOperator = QueryOperator.Default
        };
        }

        public static QueryInput ArtistQuery(string artist)
        {
            return new QueryInput
            {
                AlbumName = null,
                Artist = artist,
                Geneire = null,
                Year = null,
                Rating = null,
                RatingOperator = QueryOperator.Default
            };
        }

        public static QueryInput AlbumQuery(string album)
        {
            return new QueryInput
            {
                AlbumName = album,
                Artist = null,
                Geneire = null,
                Year = null,
                Rating = null,
                RatingOperator = QueryOperator.Default
            };
        }

        public static QueryInput GenerireQuery(string geneire)
        {
            return new QueryInput
            {
                AlbumName = null,
                Artist = null,
                Geneire = geneire,
                Year = null,
                Rating = null,
                RatingOperator = QueryOperator.Default
            };
        }

    }
}
