﻿using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class NotSupportException : Exception
    {
        public NotSupportException()
        {
        }

        public NotSupportException(string message) : base(message)
        {
        }

        public NotSupportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotSupportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}