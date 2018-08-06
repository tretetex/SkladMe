using System;
using System.IO;

namespace SkDAL
{
    public class DbLogger
    {
        public bool IsActive { get; set; }
        public string Path { get; set; } = "Diagnostic\\sql.log";
        public bool Append { get; set; }

        public DbLogger()
        {
            CreateDirectory();
        }

        private void CreateDirectory()
        {
            var fi = new FileInfo(Path);
            var dir = fi.DirectoryName;

            if (dir == null)
            {
                throw new ArgumentException("Указан недопустимый путь для sql логов.", "Path");
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
