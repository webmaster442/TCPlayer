namespace Webmaster442.LibItunesXmlDb
{
    /// <summary>
    /// Specifies filter kind for the Filter method
    /// </summary>
    public enum FilterKind
    {
        /// <summary>
        /// No filtering
        /// </summary>
        None,
        /// <summary>
        /// Filter string is an album name
        /// </summary>
        Album,
        /// <summary>
        /// Filter string is an artist name
        /// </summary>
        Artist,
        /// <summary>
        /// Filter string is a Genre
        /// </summary>
        Genre,
        /// <summary>
        /// Filter string represents a year
        /// </summary>
        Year
    }
}
