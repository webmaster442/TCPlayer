using System;
using System.IO;

namespace Mp4Chapters
{
    /// <summary>
    /// Информация о главе.
    /// </summary>
    public struct ChapterInfo
    {
        /// <summary>
        /// Время.
        /// </summary>
        public TimeSpan Time;

        /// <summary>
        /// Имя.
        /// </summary>
        public string Name;
    }
}