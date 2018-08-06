using System.Data.Entity;
using SQLite.CodeFirst;

namespace SkDAL
{
    public class UserDataContextInitializer : SqliteCreateDatabaseIfNotExists<UserDataContext>
    {
        public UserDataContextInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(UserDataContext context)
        {
        }
    }
}
