using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class NotExistException : Exception
    {
        public NotExistException()
        {
        }

        public NotExistException(string message) : base(message)
        {
        }

        public NotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}