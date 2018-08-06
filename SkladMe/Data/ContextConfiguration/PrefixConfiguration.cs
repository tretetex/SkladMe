using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;
using SkDAL.Extensions;

namespace SkDAL.ContextConfiguration
{
    public class PrefixConfiguration : EntityTypeConfiguration<Prefix>
    {
        public PrefixConfiguration()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Title).IsRequired().IsUnique();

            HasMany(prefix => prefix.Products)
                .WithRequired(product => product.Prefix)
                .HasForeignKey(product => product.PrefixId);
        }
    }
}
