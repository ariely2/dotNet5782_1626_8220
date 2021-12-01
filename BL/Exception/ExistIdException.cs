using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class ExistIdException : Exception
    {
        public ExistIdException()
        {
        }

        public ExistIdException(string message) : base(message)
        {
        }

        public ExistIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExistIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}