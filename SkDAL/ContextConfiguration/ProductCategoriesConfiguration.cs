using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;

namespace SkDAL.ContextConfiguration
{
    public class ProductCategoriesConfiguration : EntityTypeConfiguration<ProductCategories>
    {
        public ProductCategoriesConfiguration()
        {
            HasKey(p => new { p.ProductId, p.CategoryId });
        }
    }
}
