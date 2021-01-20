using System;
using System.Runtime.Serialization;

namespace Mikodev.Optional
{
    [Serializable]
    public class ResultException : Exception
    {
        public ResultException() { }

        public ResultException(string message) : base(message) { }

        public ResultException(string message, Exception inner) : base(message, inner) { }

        protected ResultException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
