﻿using System;
using System.Runtime.Serialization;

namespace IBL.BO
{
    [Serializable]
    internal class ParcelWasntPickedUpException : Exception
    {
        public ParcelWasntPickedUpException()
        {
        }

        public ParcelWasntPickedUpException(string message) : base(message)
        {
        }

        public ParcelWasntPickedUpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParcelWasntPickedUpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}