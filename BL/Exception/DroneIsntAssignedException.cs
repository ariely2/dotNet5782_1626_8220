using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    public class DroneIsntAssignedException : Exception
    {
        public DroneIsntAssignedException()
        {
        }

        public DroneIsntAssignedException(string message) : base(message)
        {
        }

        public DroneIsntAssignedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DroneIsntAssignedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}