using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class OutOfBoundsExceptoin : Exception
    {
        public OutOfBoundsExceptoin()
        {
        }

        public OutOfBoundsExceptoin(string message) : base(message)
        {
        }

        public OutOfBoundsExceptoin(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected OutOfBoundsExceptoin(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}