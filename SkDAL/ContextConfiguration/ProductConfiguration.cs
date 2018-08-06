using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SkDAL.Model;

namespace SkDAL.ContextConfiguration
{
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {
            Property(product => product.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(product => product.Title).IsRequired();
            Property(product => product.Price).IsOptional();
            Property(product => product.Fee).IsOptional();
            Property(product => product.Rating).IsOptional();
            Property(product => product.DateFee).IsOptional();

            HasRequired(product => product.Creator)
                .WithMany(user => user.ProductsAsCreator)
                .HasForeignKey(product => product.CreatorId);
            HasOptional(product => product.Organizer)
                .WithMany(user => user.ProductsAsOrganizer)
                .HasForeignKey(product => product.OrganizerId)
                .WillCascadeOnDelete(false);
            
            HasMany(product => product.MembersAsMain)
                .WithMany(user => user.ProductsAsMain)
                .Map(x =>
                {
                    x.ToTable("ProductUsers_MainList");
                    x.MapLeftKey("ProductId");
                    x.MapRightKey("UserId");
                });
            HasMany(product => product.MembersAsReserve)
                .WithMany(user => user.ProductsAsReserve)
                .Map(x =>
                {
                    x.ToTable("ProductUsers_ReserveList");
                    x.MapLeftKey("ProductId");
                    x.MapRightKey("UserId");
                });
            HasMany(product => product.UsersAsAuthor)
                .WithMany(user => user.ProductsAsAuthor)
                .Map(x =>
                {
                    x.ToTable("ProductUsers_Authors");
                    x.MapLeftKey("ProductId");
                    x.MapRightKey("UserId");
                });
        }
    }
}
