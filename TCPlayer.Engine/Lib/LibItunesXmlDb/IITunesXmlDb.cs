using System.Collections.Generic;

namespace Webmaster442.LibItunesXmlDb
{
    /// <summary>
    /// Interface for ITunesXmlDb
    /// </summary>
    public interface IITunesXmlDb
    {
        /// <summary>
        /// Gets All Tracks from the Database
        /// </summary>
        /// <seealso cref="Track"/>
        IEnumerable<Track> Tracks { get; }
        /// <summary>
        /// Gets All Album names from the Database
        /// </summary>
        IEnumerable<string> Albums { get; }
        /// <summary>
        /// Gets All Artist names from the Database
        /// </summary>
        IEnumerable<string> Artists { get; }
        /// <summary>
        /// Gets All Genres from the Database
        /// </summary>
        IEnumerable<string> Genres { get; }
        /// <summary>
        /// Gets All years from the Database
        /// </summary>
        IEnumerable<string> Years { get; }
        /// <summary>
        /// Gets All playlists from the Database
        /// </summary>
        IEnumerable<string> Playlists { get; }
        /// <summary>
        /// Filter the Tracks by a criteria
        /// </summary>
        /// <param name="kind">Specifies filter kind</param>
        /// <param name="param">Specifies Filter string</param>
        /// <returns>Tracks maching the filter kind and string</returns>
        /// <seealso cref="Track"/>
        IEnumerable<Track> Filter(FilterKind kind, string param);
        /// <summary>
        /// Gets a Playlists contents
        /// </summary>
        /// <param name="id">Plalist id</param>
        /// <returns>Tracks in the specified playlist</returns>
        /// <seealso cref="Track"/>
        IEnumerable<Track> ReadPlaylist(string id);
    }
}
