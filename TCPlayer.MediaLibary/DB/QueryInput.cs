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
using AppLib.WPF.MVVM;
using System;
using System.Xml.Serialization;

namespace TCPlayer.MediaLibary.DB
{
    [Serializable]
    public class QueryInput : BindableBase
    {
        private string _AlbumName;
        private StringOperator _AlbumNameOperator;

        private string _Artist;
        private StringOperator _ArtistOperator;

        private string _Title;
        private StringOperator _TitleOperator;

        private uint? _Year;
        private QueryOperator _YearOperator;

        private string _Geneire;
        private StringOperator _GeneireOperator;

        private string _QueryName;

        private uint? _Rating;
        private QueryOperator _RatingOperator;

        private uint? _PlayCount;
        private QueryOperator _PlayCounterOperator;

        [XmlAttribute]
        public StringOperator AlbumNameOperator
        {
            get { return _AlbumNameOperator; }
            set { SetValue(ref _AlbumNameOperator, value); }
        }

        [XmlAttribute]
        public string AlbumName
        {
            get { return _AlbumName; }
            set { SetValue(ref _AlbumName, value); }
        }

        [XmlAttribute]
        public StringOperator ArtistOperator
        {
            get { return _ArtistOperator; }
            set { SetValue(ref _ArtistOperator, value); }
        }

        [XmlAttribute]
        public string Artist
        {
            get { return _Artist; }
            set { SetValue(ref _Artist, value); }
        }

        [XmlAttribute]
        public StringOperator TitleOperator
        {
            get { return _TitleOperator; }
            set { SetValue(ref _TitleOperator, value); }
        }

        [XmlAttribute]
        public string Title
        {
            get { return _Title; }
            set { SetValue(ref _Title, value); }
        }

        [XmlAttribute]
        public QueryOperator YearOperator
        {
            get { return _YearOperator; }
            set { SetValue(ref _YearOperator, value); }
        }

        [XmlAttribute]
        public uint? Year
        {
            get { return _Year; }
            set { SetValue(ref _Year, value); }
        }

        [XmlAttribute]
        public StringOperator GeneireOperator
        {
            get { return _GeneireOperator; }
            set { SetValue(ref _GeneireOperator, value); }
        }

        [XmlAttribute]
        public string Geneire
        {
            get { return _Geneire; }
            set { SetValue(ref _Geneire, value); }
        }

        [XmlAttribute]
        public string QueryName
        {
            get { return _QueryName; }
            set { SetValue(ref _QueryName, value); }
        }

        [XmlAttribute]
        public QueryOperator RatingOperator
        {
            get { return _RatingOperator; }
            set { SetValue(ref _RatingOperator, value); }
        }

        [XmlAttribute]
        public uint? Rating
        {
            get { return _Rating; }
            set { SetValue(ref _Rating, value); }
        }

        [XmlAttribute]
        public QueryOperator PlayCounterOperator
        {
            get { return _PlayCounterOperator; }
            set { SetValue(ref _PlayCounterOperator, value); }
        }

        [XmlAttribute]
        public uint? PlayCount
        {
            get { return _PlayCount; }
            set { SetValue(ref _PlayCount, value); }
        }

        public QueryInput()
        {
            AlbumName = null;
            Artist = null;
            Year = null;
            Geneire = null;
            QueryName = null;
            Rating = null;
            ArtistOperator = StringOperator.ContainsIgnoreCase;
            TitleOperator = StringOperator.ContainsIgnoreCase;
            GeneireOperator = StringOperator.ContainsIgnoreCase;
            AlbumNameOperator = StringOperator.ContainsIgnoreCase;
            RatingOperator = QueryOperator.NotSet;
            YearOperator = QueryOperator.NotSet;
            RatingOperator = QueryOperator.NotSet;
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
                ArtistOperator = StringOperator.ContainsIgnoreCase,
                TitleOperator = StringOperator.ContainsIgnoreCase,
                GeneireOperator = StringOperator.ContainsIgnoreCase,
                AlbumNameOperator = StringOperator.ContainsIgnoreCase,
                YearOperator = QueryOperator.Equals,
                PlayCounterOperator = QueryOperator.NotSet,
                RatingOperator = QueryOperator.NotSet
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
                ArtistOperator = StringOperator.ExactmatchIgnoreCase,
                TitleOperator = StringOperator.ContainsIgnoreCase,
                GeneireOperator = StringOperator.ContainsIgnoreCase,
                AlbumNameOperator = StringOperator.ContainsIgnoreCase,
                YearOperator = QueryOperator.NotSet,
                PlayCounterOperator = QueryOperator.NotSet,
                RatingOperator = QueryOperator.NotSet
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
                ArtistOperator = StringOperator.ContainsIgnoreCase,
                TitleOperator = StringOperator.ContainsIgnoreCase,
                GeneireOperator = StringOperator.ContainsIgnoreCase,
                AlbumNameOperator = StringOperator.ExactmatchIgnoreCase,
                YearOperator = QueryOperator.NotSet,
                PlayCounterOperator = QueryOperator.NotSet,
                RatingOperator = QueryOperator.NotSet
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
                ArtistOperator = StringOperator.ContainsIgnoreCase,
                TitleOperator = StringOperator.ContainsIgnoreCase,
                GeneireOperator = StringOperator.ExactmatchIgnoreCase,
                AlbumNameOperator = StringOperator.ContainsIgnoreCase,
                YearOperator = QueryOperator.NotSet,
                PlayCounterOperator = QueryOperator.NotSet,
                RatingOperator = QueryOperator.NotSet
            };
        }

    }
}
