namespace SkServ.Exception
{
    public class UnknownMethodException : ServerApiException
    {
        public UnknownMethodException()
        {
        }

        public UnknownMethodException(string message) : base(message)
        {
        }

        public UnknownMethodException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public UnknownMethodException(string message, int code) : base(message, code)
        {
        }
    }
}
