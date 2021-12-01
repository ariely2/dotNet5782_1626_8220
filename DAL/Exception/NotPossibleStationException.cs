using System;
using System.Runtime.Serialization;

namespace IDAL.DO
{
    [Serializable]
    public class NotPossibleStationException : Exception
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