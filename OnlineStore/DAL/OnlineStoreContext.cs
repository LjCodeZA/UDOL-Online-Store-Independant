using OnlineStore.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace OnlineStore.DAL
{
    public class OnlineStoreContext : DbContext
    {
        public OnlineStoreContext() : base("OnlineStoreContext")
        {
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<ProductStock> ProductStock { get; set; }
        public DbSet<ProductVendor> ProductVendor { get; set; }
        public DbSet<Order> Order { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}