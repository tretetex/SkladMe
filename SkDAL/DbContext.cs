using System.Data.Entity;
using SkDAL.Model;

namespace SkDAL
{
    public sealed class DbContext : System.Data.Entity.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Prefix> Prefixes { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Subсhapter> Subchapters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<ProductTags> ProductTags { get; set; }

        public DbContext() : base("Sqlite")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DbContextInitializer(modelBuilder));
            modelBuilder.Configurations.AddFromAssembly(typeof(DbContext).Assembly);
        }
    }
}