using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class ExistPhoneException : Exception
    {
        public ExistPhoneException()
        {
        }

        public ExistPhoneException(string message) : base(message)
        {
        }

        public ExistPhoneException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExistPhoneException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}