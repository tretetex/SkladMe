using SkDAL;

namespace SkDAL.Config
{
    public class LogConfig
    {
        public static readonly DbLogger DbLogger = new DbLogger
        {
            IsActive = true,
            Append = false,
            Path = "Diagnostic\\sql.log"
        };
    }
}
