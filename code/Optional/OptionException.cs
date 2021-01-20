using System;
using System.Runtime.Serialization;

namespace Mikodev.Optional
{
    [Serializable]
    public class OptionException : Exception
    {
        public OptionException() { }

        public OptionException(string message) : base(message) { }

        public OptionException(string message, Exception inner) : base(message, inner) { }

        protected OptionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
