namespace SkladchikGet.Exception
{
    public static class ExceptionExtensions
    {
        public static System.Exception GetOriginalException(this System.Exception ex)
        {
            return ex.InnerException == null ? ex : ex.InnerException.GetOriginalException();
        }
    }
}
