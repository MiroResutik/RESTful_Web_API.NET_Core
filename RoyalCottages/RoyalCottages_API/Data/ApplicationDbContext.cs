using Microsoft.EntityFrameworkCore;
using RoyalCottages_API.Models;

namespace RoyalCottages_API.Data
{
    // Define DbSet properties for your entities
    // Accepting dbContextOptions to configure the database connection and other options
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Cottage> Cottage { get; set; }
    }
}
