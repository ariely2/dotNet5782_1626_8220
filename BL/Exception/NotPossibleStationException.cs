using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class NotPossibleStationException : Exception
    {
        public NotPossibleStationException()
        {
        }

        public NotPossibleStationException(string message) : base(message)
        {
        }

        public NotPossibleStationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotPossibleStationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}