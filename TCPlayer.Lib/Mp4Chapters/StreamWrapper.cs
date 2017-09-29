using System.IO;

namespace Mp4Chapters
{
    /// <summary>
    /// Обёртка для потока.
    /// </summary>
    public sealed class StreamWrapper : IAbstractStream
    {
        private readonly Stream stream;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="stream">Поток.</param>
        public StreamWrapper(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Длина.
        /// </summary>
        public long Length
        {
            get { return stream.Length; }
        }

        /// <summary>
        /// Позиция.
        /// </summary>
        public long Position
        {
            get { return stream.Position; }
        }

        /// <summary>
        /// Перейти к позиции.
        /// </summary>
        /// <param name="origin">Откуда смещение.</param>
        /// <param name="offset">Смещение.</param>
        public void Seek(SeekOrigin origin, long offset)
        {
            stream.Seek(offset, origin);
        }

        /// <summary>
        /// Читать.
        /// </summary>
        /// <param name="buffer">Буфер.</param>
        /// <param name="bytes">Байт.</param>
        /// <returns>Количество прочитанных байт.</returns>
        public int Read(byte[] buffer, int bytes)
        {
            return stream.Read(buffer, 0, bytes);
        }
    }
}