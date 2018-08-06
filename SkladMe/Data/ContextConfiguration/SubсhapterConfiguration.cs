using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;

namespace SkDAL.ContextConfiguration
{
    public class SubсhapterConfiguration : EntityTypeConfiguration<Subсhapter>
    {
        public SubсhapterConfiguration()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Title).IsRequired();

            HasMany(subсhapter => subсhapter.Products)
                .WithRequired(product => product.Subсhapter)
                .HasForeignKey(product => product.SubchapterId);
        }
    }
}
