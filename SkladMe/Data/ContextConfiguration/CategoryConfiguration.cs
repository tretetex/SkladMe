using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;
using SkDAL.Extensions;

namespace SkDAL.ContextConfiguration
{
    public class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            Property(p => p.Title).IsRequired();

            HasOptional(p => p.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(p => p.ParentId)
                .WillCascadeOnDelete(true);
        }
    }
}
