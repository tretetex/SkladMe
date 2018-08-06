using System.Data.Entity.Migrations;
using SkDAL.Model;
using SkDAL.Seeds.Base;

namespace SkDAL.Seeds
{
    public class ChaptersTableSeeder : Seeder
    {
        public ChaptersTableSeeder(DbContext db) : base(db)
        {
        }

        /// <summary>
        /// ��������� ���������� ������� Chapters
        /// </summary>
        protected override void Run()
        {
            Db.Chapters.AddOrUpdate(new Chapter { Id = 105, Title = "��������� ���������" });
            Db.Chapters.AddOrUpdate(new Chapter { Id = 106, Title = "��������" });
            Db.Chapters.AddOrUpdate(new Chapter { Id = 15, Title = "���������" });
        }
    }
}