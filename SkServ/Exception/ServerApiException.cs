using SkServ.Utils;

namespace SkServ.Exception
{
    public class ServerApiException : System.Exception
    {
        public int ErrorCode { get; internal set; }
        public RequestParameters RequestParams { get; set; }

        public ServerApiException()
        {
        }

        public ServerApiException(string message) : base(message)
        {
        }

        public ServerApiException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public ServerApiException(string message, int code) : base(message)
        {
            ErrorCode = code;
        }

        public ServerApiException(string message, int code, System.Exception innerException) : base(message, innerException)
        {
            ErrorCode = code;
        }
    }
}
