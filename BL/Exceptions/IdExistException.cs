using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    public class IdExistException : Exception
    {
        public IdExistException()
        {
        }

        public IdExistException(string message) : base(message)
        {
        }

        public IdExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}