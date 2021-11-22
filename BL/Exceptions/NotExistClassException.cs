using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class NotExistClassException : Exception
    {
        public NotExistClassException()
        {
        }

        public NotExistClassException(string message) : base(message)
        {
        }

        public NotExistClassException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotExistClassException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}