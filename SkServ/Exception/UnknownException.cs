namespace SkServ.Exception
{
    public class UnknownException : ServerApiException
    {
        public UnknownException()
        {
        }

        public UnknownException(string message) : base(message)
        {
        }

        public UnknownException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public UnknownException(string message, int code) : base(message, code)
        {
        }
    }
}
