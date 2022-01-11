using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    internal class DroneIsntChargeException : Exception
    {
        public DroneIsntChargeException()
        {
        }

        public DroneIsntChargeException(string message) : base(message)
        {
        }

        public DroneIsntChargeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DroneIsntChargeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}