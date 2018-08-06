using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using SkDAL.Model;

namespace SkDAL
{
    public sealed class UserDataContext : System.Data.Entity.DbContext
    {
        private static readonly DatabaseLogger DatabaseLogger = new DatabaseLogger(DbLogger.PathUserData, DbLogger.Append);

        public DbSet<ProductInfo> ProductInfo { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }

        public UserDataContext() : base("UserData")
        {
            if (DbLogger.IsActive)
            {
                DatabaseLogger.StartLogging();
                DbInterception.Add(DatabaseLogger);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer(new UserDataContextInitializer(modelBuilder));
            modelBuilder.Configurations.AddFromAssembly(typeof(UserDataContext).Assembly);
        }

        protected override void Dispose(bool disposing)
        {
            if (DbLogger.IsActive)
            {
                DbInterception.Remove(DatabaseLogger);
                DatabaseLogger.StopLogging();
            }
            base.Dispose(disposing);
        }
    }
}