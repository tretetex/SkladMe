using System.Data.Entity;
using Cookie = SkDAL.Model.Cookie;

namespace SkDAL
{
    
    public class DbCookiesContext : System.Data.Entity.DbContext
    {
        public DbCookiesContext() : base("Cookies")
        {
        }
        
        public DbSet<Cookie> Cookies { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DbCookiesContextInitializer(modelBuilder));
            base.OnModelCreating(modelBuilder);
        }
    }
}
