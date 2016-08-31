using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BassPlayer2.Code
{
    internal static class Extensions
    {
        /// <summary>
        /// Converts a Timespan to a nice formated string
        /// </summary>
        /// <param name="ts">timespan to format</param>
        /// <returns>returns timespan in the folllowing format: hh:mm:ss</returns>
        public static string ToShortTime(this TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        /// <summary>
        /// Converts an IEnumerable to an Observable collection
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="coll">an IEnumerable collection</param>
        /// <returns>The elements in an ObservableCollection</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in coll) c.Add(e);
            return c;
        }

        /// <summary>
        /// Apends elements from an IEnumerable collection to an observable collection
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="collection">The ObserbableCollection to apend to</param>
        /// <param name="elements">an IEnumerable collection</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> elements)
        {
            if (elements == null) return;
            foreach (var e in elements) collection.Add(e);
        }
    }
}
