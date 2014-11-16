using System;

namespace CherryCouch.Exceptions
{
    public class ForbiddenRequestException : Exception
    {
        public ForbiddenRequestException(string details)
            : base(String.Format("Forbidden access : {0}", details))
        {
        }
    }
}
