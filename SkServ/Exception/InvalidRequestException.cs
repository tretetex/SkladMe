namespace SkServ.Exception
{
    public class InvalidRequestException : ServerApiException
    {
        public InvalidRequestException()
        {
        }

        public InvalidRequestException(string message) : base(message)
        {
        }

        public InvalidRequestException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public InvalidRequestException(string message, int code) : base(message, code)
        {
        }
    }
}
