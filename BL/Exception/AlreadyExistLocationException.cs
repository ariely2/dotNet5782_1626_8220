using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class AlreadyExistLocationException : Exception
    {
        public AlreadyExistLocationException()
        {
        }

        public AlreadyExistLocationException(string message) : base(message)
        {
        }

        public AlreadyExistLocationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyExistLocationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}