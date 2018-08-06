using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;
using SkDAL.Extensions;

namespace SkDAL.ContextConfiguration
{
    public class ChapterConfiguration : EntityTypeConfiguration<Chapter>
    {
        public ChapterConfiguration()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Title).IsRequired().IsUnique();

            HasMany(chapter => chapter.Products)
                .WithRequired(product => product.Chapter)
                .HasForeignKey(product => product.ChapterId);
        }
    }
}
