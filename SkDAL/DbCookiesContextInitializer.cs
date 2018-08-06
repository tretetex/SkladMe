using System.Data.Entity;
using SQLite.CodeFirst;

namespace SkDAL
{
    public class DbCookiesContextInitializer : SqliteCreateDatabaseIfNotExists<DbCookiesContext>
    {
        public DbCookiesContextInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(DbCookiesContext db)
        {
        }
    }
}
