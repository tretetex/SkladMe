using System.Data.Entity.ModelConfiguration;
using SkDAL.EFUtilities.Extensions;
using SkDAL.Model;

namespace SkDAL.ContextConfiguration
{
    public class TagConfiguration : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            Property(p => p.Title).IsRequired().IsUnique();
        }
    }
}
