using System.Data.Entity;
using SkDAL.Seeds;
using SQLite.CodeFirst;

namespace SkDAL
{
    public class DbContextInitializer : SqliteCreateDatabaseIfNotExists<DbContext>
    {
        public DbContextInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(DbContext db)
        {
            new ChaptersTableSeeder(db).Call();
            new PrefixesTableSeeder(db).Call();
            new SubchaptersTableSeeder(db).Call();
            new UserGroupsTableSeeder(db).Call();

            db.SaveChanges();
        }
    }
}
