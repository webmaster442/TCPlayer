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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TCPlayer.Code
{
    internal static class Extensions
    {
        /// <summary>
        /// Apends elements from an IEnumerable collection to an observable collection
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="collection">The ObserbableCollection to apend to</param>
        /// <param name="elements">an IEnumerable collection</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null || items == null || !items.Any())
                return;

            Type type = collection.GetType();

            var bindflags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic;

            type.InvokeMember("CheckReentrancy", bindflags, null, collection, null);

            var itemsProp = type.BaseType.GetProperty("Items", BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

            var privateItems = itemsProp.GetValue(collection) as IList<T>;

            foreach (var item in items)
            {
                privateItems.Add(item);
            }

            type.InvokeMember("OnPropertyChanged", bindflags, null,
              collection, new object[] { new PropertyChangedEventArgs("Count") });

            type.InvokeMember("OnPropertyChanged", bindflags, null,
              collection, new object[] { new PropertyChangedEventArgs("Item[]") });

            type.InvokeMember("OnCollectionChanged", bindflags, null,
              collection, new object[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) });
        }

        /// <summary>
        /// Converts an IEnumerable to an Observable collection
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="coll">an IEnumerable collection</param>
        /// <returns>The elements in an ObservableCollection</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>(coll);
            return c;
        }

        /// <summary>
        /// Converts a Timespan to a nice formated string
        /// </summary>
        /// <param name="ts">timespan to format</param>
        /// <returns>returns timespan in the folllowing format: hh:mm:ss</returns>
        public static string ToShortTime(this TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }
    }
}
