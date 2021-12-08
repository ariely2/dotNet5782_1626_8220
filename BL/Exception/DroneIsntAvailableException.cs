using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    public class DroneIsntAvailableException : Exception
    {
        public DroneIsntAvailableException()
        {
        }

        public DroneIsntAvailableException(string message) : base(message)
        {
        }

        public DroneIsntAvailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DroneIsntAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}