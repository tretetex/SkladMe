using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SkDAL.EFUtilities.Extensions;
using SkDAL.Model;

namespace SkDAL.ContextConfiguration
{
    public class UserGroupConfiguration : EntityTypeConfiguration<UserGroup>
    {
        public UserGroupConfiguration()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Title).IsRequired().IsUnique();
        }
    }
}
