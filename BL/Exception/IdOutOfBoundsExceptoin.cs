using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class IdOutOfBoundsExceptoin : Exception
    {
        public IdOutOfBoundsExceptoin()
        {
        }

        public IdOutOfBoundsExceptoin(string message) : base(message)
        {
        }

        public IdOutOfBoundsExceptoin(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected IdOutOfBoundsExceptoin(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}