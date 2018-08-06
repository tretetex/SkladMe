namespace SkServ.Exception
{
    public class InvalidLicenseException : ServerApiException
    {
        public InvalidLicenseException()
        {
        }

        public InvalidLicenseException(string message) : base(message)
        {
        }

        public InvalidLicenseException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public InvalidLicenseException(string message, int code) : base(message, code)
        {
        }
    }
}
