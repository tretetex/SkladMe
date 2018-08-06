using System.Data.Entity.Migrations;
using SkDAL.Model;
using SkDAL.Seeds.Base;

namespace SkDAL.Seeds
{
    public class UserGroupsTableSeeder : Seeder
    {
        public UserGroupsTableSeeder(DbContext db) : base(db)
        {
        }

        protected override void Run()
        {
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 1, Title = "Администратор" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 2, Title = "Складчик" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 4, Title = "Модератор" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 24, Title = "Организатор" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 25, Title = "Партнер" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 26, Title = "Член клуба" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 27, Title = "Штрафник" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 29, Title = "Забанен" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 30, Title = "Резервист" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 37, Title = "Робот" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 40, Title = "Партнер (А)" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 41, Title = "Партнер (П)" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 42, Title = "Член клуба (А)" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 43, Title = "Член клуба (П)" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 44, Title = "Организатор (А)" });
            Db.UserGroups.AddOrUpdate(new UserGroup { Id = 45, Title = "Организатор (П)" });
        }
    }
}