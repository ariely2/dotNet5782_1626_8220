using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    public class ExistObjectIdException : Exception
    {
        public ExistObjectIdException()
        {
        }

        public ExistObjectIdException(string message) : base(message)
        {
        }

        public ExistObjectIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExistObjectIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}