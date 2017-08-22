using System;
using System.Runtime.Serialization;
using System.Text;

namespace TCPlayer.MediaLibary.DB
{
    [Serializable]
    public class DBException : Exception
    {
        public DBException()
        {
        }

        public DBException(string message) : base(message)
        {
        }

        public DBException(StringBuilder message) : base(message.ToString())
        {
        }

        public DBException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DBException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
