using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;
using SkDAL.Extensions;

namespace SkDAL.ContextConfiguration
{
    public class TagConfiguration : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            Property(p => p.Title).IsRequired().IsUnique();

            HasMany(tag => tag.Products)
                .WithMany(product => product.Tags)
                .Map(x =>
                {
                    x.ToTable("ProductTags");
                    x.MapLeftKey("ProductId");
                    x.MapRightKey("TagId");
                });
        }
    }
}
