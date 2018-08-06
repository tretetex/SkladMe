using System;
using System.IO;

namespace SkDAL
{
    public static class DbLogger
    {
        public static bool IsActive { get; } = false;
        public static string Path { get; } = "logs\\sql.log";
        public static string PathUserData { get; } = "logs\\user-data.sql.log";
        public static bool Append { get; } = false;

        static DbLogger()
        {
            CreateDirectory(Path);
            CreateDirectory(PathUserData);
        }

        private static void CreateDirectory(string path)
        {
            var fi = new FileInfo(path);
            var dir = fi.DirectoryName;

            if (dir == null)
            {
                throw new ArgumentException("Указан недопустимый путь для sql логов.", nameof(path));
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
