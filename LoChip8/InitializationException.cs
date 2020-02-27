using System;
using System.Runtime.Serialization;

namespace LoChip8
{
    [Serializable]
    public class InitializationException : Exception
    {
        public InitializationException()
        {
        }

        public InitializationException(string message) : base(message)
        {
        }

        public InitializationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InitializationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}