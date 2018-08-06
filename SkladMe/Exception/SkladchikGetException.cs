namespace SkladMe.Exception
{
    using System;

    public class SkladchikGetException : Exception
    {
        public SkladchikGetException()
        {
        }

        public SkladchikGetException(string message) : base(message)
        {
        }

        public SkladchikGetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}