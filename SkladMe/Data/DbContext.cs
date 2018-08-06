using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using SkDAL.Config;
using SkDAL.Model;

namespace SkDAL
{
    public sealed class DbContext : System.Data.Entity.DbContext
    {
        private static readonly DatabaseLogger DatabaseLogger = new DatabaseLogger(LogConfig.DbLogger.Path,
            LogConfig.DbLogger.Append);

        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Prefix> Prefixes { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Subсhapter> Subchapters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }

        public DbContext() : base("Sqlite")//base(CreateConnection(), false)
        {
            ConfigureLog();
        }

        private void ConfigureLog()
        {
            if (LogConfig.DbLogger.IsActive)
            {
                DatabaseLogger.StartLogging();
                DbInterception.Add(DatabaseLogger);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DbContextInitializer(modelBuilder));
            modelBuilder.Configurations.AddFromAssembly(typeof(DbContext).Assembly);
        }

        public static SQLiteConnection CreateConnection()
        {
            var builder = (SQLiteConnectionStringBuilder)SQLiteProviderFactory.Instance.CreateConnectionStringBuilder();
            if (builder == null)
            {
                return null;
            }
            //builder.DataSource = path;
            builder.FailIfMissing = false;
            builder.ForeignKeys = true;

            return new SQLiteConnection(builder.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (LogConfig.DbLogger.IsActive)
            {
                DbInterception.Remove(DatabaseLogger);
                DatabaseLogger.StopLogging();
            }
            base.Dispose(disposing);
        }
    }
}