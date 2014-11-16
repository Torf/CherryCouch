using System;

namespace CherryCouch.Exceptions
{
    public class RequestFormatException : Exception
    {
        public RequestFormatException(string error)
            : base(String.Format("invalid request format : {0}", error))
        {
        }
    }
}
