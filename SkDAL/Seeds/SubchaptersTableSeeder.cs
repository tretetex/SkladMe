using System.Data.Entity.Migrations;
using SkDAL.Model;
using SkDAL.Seeds.Base;

namespace SkDAL.Seeds
{
    public class SubchaptersTableSeeder : Seeder
    {
        public SubchaptersTableSeeder(DbContext db) : base(db)
        {
        }

        protected override void Run()
        {
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 107, ChapterId = 105, Title = "Бизнес и свое дело" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 108, ChapterId = 105, Title = "Дизайн и креатив" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 109, ChapterId = 105, Title = "Здоровье и быт" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 110, ChapterId = 105, Title = "Психология и отношения" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 111, ChapterId = 105, Title = "Хобби и увлечения" });

            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 112, ChapterId = 106, Title = "Программирование" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 113, ChapterId = 106, Title = "Бизнес и свое дело" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 114, ChapterId = 106, Title = "Дизайн и креатив" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 115, ChapterId = 106, Title = "Здоровье и быт" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 116, ChapterId = 106, Title = "Психология и отношения" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 117, ChapterId = 106, Title = "Хобби и увлечения" });

            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 21, ChapterId = 15, Title = "Курсы по программированию" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 16, ChapterId = 15, Title = "Курсы по администрированию" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 24, ChapterId = 15, Title = "Курсы по бизнесу" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 103, ChapterId = 15, Title = "Бухгалтерия и финансы" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 26, ChapterId = 15, Title = "Курсы по SEO и SMM" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 19, ChapterId = 15, Title = "Курсы по дизайну" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 78, ChapterId = 15, Title = "Курсы по фото и видео" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 60, ChapterId = 15, Title = "Курсы по музыке" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 30, ChapterId = 15, Title = "Электронные книги" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 71, ChapterId = 15, Title = "Курсы по здоровью" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 118, ChapterId = 15, Title = "Курсы по самообороне" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 97, ChapterId = 15, Title = "Отдых и путешествия" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 38, ChapterId = 15, Title = "Курсы по психологии" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 98, ChapterId = 15, Title = "Курсы по эзотерике" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 59, ChapterId = 15, Title = "Курсы по соблазнению" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 102, ChapterId = 15, Title = "Имидж и стиль" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 95, ChapterId = 15, Title = "Дети и родители" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 104, ChapterId = 15, Title = "Школа и репетиторство" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 99, ChapterId = 15, Title = "Хобби и рукоделие" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 94, ChapterId = 15, Title = "Строительство и ремонт" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 101, ChapterId = 15, Title = "Сад и огород" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 100, ChapterId = 15, Title = "Авто-мото" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 32, ChapterId = 15, Title = "Скрипты и программы" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 82, ChapterId = 15, Title = "Шаблоны и темы" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 58, ChapterId = 15, Title = "Базы и каталоги" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 36, ChapterId = 15, Title = "Покер, ставки, казино" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 69, ChapterId = 15, Title = "Спортивные события" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 37, ChapterId = 15, Title = "Форекс и инвестиции" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 31, ChapterId = 15, Title = "Доступ к платным ресурсам" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 83, ChapterId = 15, Title = "Иностранные языки" });
            Db.Subchapters.AddOrUpdate(new Subсhapter { Id = 28, ChapterId = 15, Title = "Разные аудио и видеокурсы" });
        }
    }
}