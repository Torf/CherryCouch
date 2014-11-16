using System;

namespace CherryCouch.Exceptions
{
    public class MissingParameterRequestException : RequestFormatException
    {
        public MissingParameterRequestException(string parameterName)
            : base(String.Format("missing parameter {0}", parameterName))
        {
        }
    }
}
