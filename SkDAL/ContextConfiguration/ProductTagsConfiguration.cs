using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;

namespace SkDAL.ContextConfiguration
{
    public class ProductTagsConfiguration : EntityTypeConfiguration<ProductTags>
    {
        public ProductTagsConfiguration()
        {
            HasKey(p => new { p.ProductId, p.TagId });
            Property(x => x.ProductId).HasColumnName("TagId");
            Property(x => x.TagId).HasColumnName("ProductId");
            ToTable("ProductTags");
        }
    }
}
