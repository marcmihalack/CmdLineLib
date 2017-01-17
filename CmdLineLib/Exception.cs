using System;
using System.Runtime.Serialization;

namespace CmdLineLib
{
    [Serializable]
    public class CmdLineException : Exception
    {
        public CmdLineException() : base()
        {
        }

        public CmdLineException(string message) : base(message)
        {
        }

        protected CmdLineException(SerializationInfo info, StreamingContext context)
            : base(info,context)
        { 
        }

        public CmdLineException(string message, Exception innerException)
            : base(message,innerException)
        {
        }
    }

    [Serializable]
    public class CmdLineArgException : CmdLineException
    {
        public string ArgName { get; }

        public CmdLineArgException(string argName, string message) : base(message)
        {
            ArgName = argName;
        }

        protected CmdLineArgException(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {
        }

        public CmdLineArgException(string argName, string message, Exception innerException)
            : base(message,innerException)
        {
            ArgName = argName;
        }
    }
}
