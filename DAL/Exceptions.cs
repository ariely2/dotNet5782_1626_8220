using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
      
        [Serializable]
        public class NotExistId : Exception
        {
            public NotExistId()
            {
            }

            public NotExistId(string message) : base(message)
            {
            }
            public NotExistId(string message, Exception innerException) : base(message, innerException)
            {
            }
            protected NotExistId(SerializationInfo info, StreamingContext context) : base(info, context)
            {

            }
        }
        
        public class NotExistStruct : Exception
        {
            public NotExistStruct()
            {
            }

            public NotExistStruct(string message) : base(message)
            {
            }

            public NotExistStruct(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected NotExistStruct(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        internal class ExistId : Exception
        {
            public ExistId()
            {
            }

            public ExistId(string message) : base(message)
            {
            }

            public ExistId(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected ExistId(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
