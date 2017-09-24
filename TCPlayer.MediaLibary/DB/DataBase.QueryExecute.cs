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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLib.Common.Extensions;

namespace TCPlayer.MediaLibary.DB
{
    public sealed partial class DataBase
    {
        private bool StringSearchPredicate(string str1, string str2,  StringOperator op)
        {
            switch (op)
            {
                case StringOperator.Contains:
                    return str1.Contains(str2);
                case StringOperator.ContainsIgnoreCase:
                    return str1.Contains(str2, StringComparison.InvariantCultureIgnoreCase);
                case StringOperator.Exactmatch:
                    return str1 == str2;
                case StringOperator.ExactmatchIgnoreCase:
                    return String.Compare(str1, str2, true) == 0;
                default:
                    return false;
            }
        }

        public bool UIntSearchPredicate(uint int1, uint int2, QueryOperator op)
        {
            switch (op)
            {
                case QueryOperator.Equals:
                    return int1 == int2;
                case QueryOperator.Greater:
                    return int1 > int2;
                case QueryOperator.GreaterOrEqual:
                    return int1 >= int2;
                case QueryOperator.Less:
                    return int1 < int2;
                case QueryOperator.LessOrEqual:
                    return int1 <= int2;
                case QueryOperator.NotSet:
                default:
                    return false;
            }
        }

        public IEnumerable<TrackEntity> Execute(QueryInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var db = _tracks.FindAll();
            IEnumerable<TrackEntity> results = null;

            if (!string.IsNullOrEmpty(input.AlbumName))
            {
                results = db.Where(item => StringSearchPredicate(item.Album, input.AlbumName, input.AlbumNameOperator));
            }

            if (!string.IsNullOrEmpty(input.Artist))
            {
                if (results == null)
                    results = db.Where(item => StringSearchPredicate(item.Artist, input.Artist, input.ArtistOperator));
                else
                    results = results.Where(item => StringSearchPredicate(item.Artist, input.Artist, input.ArtistOperator));
            }

            if (!string.IsNullOrEmpty(input.Title))
            {
                if (results == null)
                    results = db.Where(item => StringSearchPredicate(item.Title, input.Title, input.TitleOperator));
                else
                    results = results.Where(item => StringSearchPredicate(item.Title, input.Title, input.TitleOperator));
            }

            if (!string.IsNullOrEmpty(input.Geneire))
            {
                if (results == null)
                    results = db.Where(item => StringSearchPredicate(item.Generire, input.Geneire, input.GeneireOperator));
                else
                    results = results.Where(item => StringSearchPredicate(item.Generire, input.Geneire, input.GeneireOperator));
            }

            if (input.Year != null)
            {
                if (results == null)
                    results = db.Where(item => UIntSearchPredicate(item.Year, input.Year.Value, input.YearOperator));
                else
                    results = results.Where(item => UIntSearchPredicate(item.Year, input.Year.Value, input.YearOperator));
            }



            return results;
        }
    }
}
