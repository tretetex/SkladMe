using System.Data.Entity.Migrations;
using SkDAL.Model;
using SkDAL.Seeds.Base;

namespace SkDAL.Seeds
{
    public class PrefixesTableSeeder : Seeder
    {
        public PrefixesTableSeeder(DbContext db) : base(db)
        {
        }

        protected override void Run()
        {
            Db.Prefixes.AddOrUpdate(new Prefix { Id = 1, Title = "Открыто" });
            Db.Prefixes.AddOrUpdate(new Prefix { Id = 2, Title = "Активно" });
            Db.Prefixes.AddOrUpdate(new Prefix { Id = 3, Title = "Доступно" });
            Db.Prefixes.AddOrUpdate(new Prefix { Id = 4, Title = "Закрыто" });
        }
    }
}