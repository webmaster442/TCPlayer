using System;
using System.Collections.Generic;

namespace Webmaster442.LibItunesXmlDb
{
    /// <summary>
    /// A class representing various parser options
    /// </summary>
    public class ITunesXmlDbOptions : IEquatable<ITunesXmlDbOptions>
    {
        /// <summary>
        /// Exclude tracks that don't exist on the user's system
        /// Default value is false
        /// </summary>
        public bool ExcludeNonExistingFiles { get; set; }

        /// <summary>
        /// Enable or Disable paralel track parsing.
        /// Default value is true
        /// </summary>
        public bool ParalelParsingEnabled { get; set; }

        /// <summary>
        /// Creates a new instance of ITunesXmlDbOptions
        /// </summary>
        public ITunesXmlDbOptions()
        {
            ParalelParsingEnabled = true;
            ExcludeNonExistingFiles = false;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as ITunesXmlDbOptions);
        }

        /// <inheritdoc/>
        public bool Equals(ITunesXmlDbOptions other)
        {
            return other != null &&
                   ExcludeNonExistingFiles == other.ExcludeNonExistingFiles &&
                   ParalelParsingEnabled == other.ParalelParsingEnabled;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 1323143269;
            hashCode = hashCode * -1521134295 + ExcludeNonExistingFiles.GetHashCode();
            hashCode = hashCode * -1521134295 + ParalelParsingEnabled.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Compares two ITunesXmlDbOptions for equality
        /// </summary>
        /// <param name="options1">instance to compare</param>
        /// <param name="options2">other instance to compare</param>
        /// <returns>true, if the instances are equal, false if not</returns>
        public static bool operator ==(ITunesXmlDbOptions options1, ITunesXmlDbOptions options2)
        {
            return EqualityComparer<ITunesXmlDbOptions>.Default.Equals(options1, options2);
        }

        /// <summary>
        /// Compares two ITunesXmlDbOptions for inequality
        /// </summary>
        /// <param name="options1">instance to compare</param>
        /// <param name="options2">other instance to compare</param>
        /// <returns>true, if the instances are not equal, false if they are</returns>
        public static bool operator !=(ITunesXmlDbOptions options1, ITunesXmlDbOptions options2)
        {
            return !(options1 == options2);
        }
    }
}
