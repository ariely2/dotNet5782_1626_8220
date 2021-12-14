using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class AlreadyPickedUpException : Exception
    {
        public AlreadyPickedUpException()
        {
        }

        public AlreadyPickedUpException(string message) : base(message)
        {
        }

        public AlreadyPickedUpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyPickedUpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}