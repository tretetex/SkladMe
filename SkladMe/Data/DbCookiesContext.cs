using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cookie = SkDAL.Model.Cookie;
using SkDAL;

namespace SkDAL
{
    
    class DbCookiesContext : System.Data.Entity.DbContext
    {
        public static string DbPath { get; set; } = Directory.GetCurrentDirectory() + "\\Cache\\Cookies";

        public DbCookiesContext() : base(CreateConnection(DbPath), false)
        {

        }
        public DbSet<Model.Cookie> Cookies { get; set; }

        public static SQLiteConnection CreateConnection(string path)
        {
            var builder = (SQLiteConnectionStringBuilder)SQLiteProviderFactory.Instance.CreateConnectionStringBuilder();
            if (builder == null)
            {
                return null;
            }

            builder.DataSource = path;
            builder.FailIfMissing = false;
            builder.ForeignKeys = true;

            return new SQLiteConnection(builder.ToString());
        }
    }
}
