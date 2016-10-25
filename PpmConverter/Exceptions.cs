using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{

    [Serializable]
    public class IllegalFormatException : Exception
    {
        public IllegalFormatException() { }
        public IllegalFormatException(string message) : base(message) { }
        public IllegalFormatException(string message, Exception inner) : base(message, inner) { }
        protected IllegalFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }

    [Serializable]
    public class WrongExtensionException : Exception
    {
        public WrongExtensionException() { }
        public WrongExtensionException(string message) : base(message) { }
        public WrongExtensionException(string message, Exception inner) : base(message, inner) { }
        protected WrongExtensionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
