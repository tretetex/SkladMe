using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;

namespace SkDAL.ContextConfiguration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(user => user.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(user => user.LastActivityDate).IsOptional();
            Property(user => user.RegistrationDate).IsOptional();
            Property(user => user.PaidMainListCount).IsOptional();
            Property(user => user.PaidTotalCount).IsOptional();
            Property(user => user.OrganizedCount).IsOptional();
            Property(user => user.MessagesCount).IsOptional();
            Property(user => user.LikesCount).IsOptional();
            Property(user => user.UserGroupId).IsOptional();
        }
    }
}
