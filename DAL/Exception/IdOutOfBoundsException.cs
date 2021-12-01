using System;
using System.Runtime.Serialization;

namespace IDAL.DO
{
    [Serializable]
    public class IdOutOfBoundsException : Exception
    {
        public IdOutOfBoundsException()
        {
        }

        public IdOutOfBoundsException(string message) : base(message)
        {
        }

        public IdOutOfBoundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdOutOfBoundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}