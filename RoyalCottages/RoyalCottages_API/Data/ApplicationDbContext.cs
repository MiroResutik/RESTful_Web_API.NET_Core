using Microsoft.EntityFrameworkCore;
using RoyalCottages_API.Models;

namespace RoyalCottages_API.Data
{
    // Define DbSet properties for your entities
    // Accepting dbContextOptions to configure the database connection and other options
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Cottage> Cottage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cottage>().HasData(


            new Cottage
            {
                Id = 1,
                Name = "Royal Cottage",
                Details = "Experience a relaxing coastal retreat with breathtaking ocean views, spacious living areas, and exclusive access to a private beach.",
                Rate = 500.0,
                SqMeters = 250,
                Occupancy = 6,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1)
            },
            new Cottage
            {
                Id = 2,
                Name = "Diamond Cottage",
                Details = "Enjoy an elegant mountain getaway featuring refined marble interiors, generous living spaces, and spectacular panoramic mountain views.",
                Rate = 750.0,
                SqMeters = 320,
                Occupancy = 8,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
                CreatedDate = new DateTime(2026, 1, 15),
                UpdatedDate = new DateTime(2026, 1, 15)
            },
            new Cottage
            {
                Id = 3,
                Name = "Pool Cottage",
                Details = "Relax in a contemporary retreat with a stunning infinity pool, stylish interiors, and a private outdoor area perfect for entertaining.",
                Rate = 350.0,
                SqMeters = 180,
                Occupancy = 4,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                CreatedDate = new DateTime(2026, 2, 1),
                UpdatedDate = new DateTime(2026, 2, 1)
            },
            new Cottage
            {
                Id = 4,
                Name = "Luxury Cottage",
                Details = "Indulge in an exceptional luxury escape with expansive interiors, premium amenities, spa facilities, and personalised concierge services.",
                Rate = 900.0,
                SqMeters = 4000,
                Occupancy = 10,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
                CreatedDate = new DateTime(2026, 2, 14),
                UpdatedDate = new DateTime(2026, 2, 14)
            },
            new Cottage
            {
                Id = 5,
                Name = "Garden Cottage",
                Details = "Unwind in a peaceful countryside hideaway surrounded by lush gardens, natural scenery, and scenic walking trails, ideal for a quiet escape.",
                Rate = 275.0,
                SqMeters = 150,
                Occupancy = 3,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
                CreatedDate = new DateTime(2026, 3, 1),
                UpdatedDate = new DateTime(2026, 3, 1)
            }


            );
        }
    }
}
