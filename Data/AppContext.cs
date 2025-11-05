using Microsoft.EntityFrameworkCore;
using StanaGO.Models;

namespace StanaGO.Data
{
    public class AppContext : DbContext
    {
        public AppContext ( DbContextOptions<AppContext> options ) : base (options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shepherd> Shepards { get; set; }
        public DbSet<Product> Products { get; set; }
    }

}
