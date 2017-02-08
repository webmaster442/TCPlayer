using System.IO;

namespace Mp4Chapters
{
    /// <summary>
    /// Абстрактный поток.
    /// </summary>
    public interface IAbstractStream
    {
        /// <summary>
        /// Длина.
        /// </summary>
        long Length { get; } 

        /// <summary>
        /// Позиция.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Перейти к позиции.
        /// </summary>
        /// <param name="origin">Откуда смещение.</param>
        /// <param name="offset">Смещение.</param>
        void Seek(SeekOrigin origin, long offset);

        /// <summary>
        /// Читать.
        /// </summary>
        /// <param name="buffer">Буфер.</param>
        /// <param name="bytes">Байт.</param>
        /// <returns>Количество прочитанных байт.</returns>
        int Read(byte[] buffer, int bytes);
    }
}