using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class NotExistIdException : Exception
    {
        public NotExistIdException()
        {
        }

        public NotExistIdException(string message) : base(message)
        {
        }

        public NotExistIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotExistIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}