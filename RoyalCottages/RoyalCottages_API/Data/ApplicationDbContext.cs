using Microsoft.EntityFrameworkCore;
using RoyalCottages_API.Models;

namespace RoyalCottages_API.Data
{
    // Define DbSet properties for your entities
    // Accepting dbContextOptions to configure the database connection and other options
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        // Define DbSet properties Cottage entity
        public DbSet<Cottage> Cottage { get; set; }

        // Define DbSet properties for User entity
        public DbSet<User> Users { get; set; }

        // Override OnModelCreating to configure the model and seed initial data
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
            },
                        new Cottage
                        {
                            Id = 6,
                            Name = "Seaside Cottage",
                            Details = "Wake up to refreshing sea breezes in this charming coastal retreat, featuring bright interiors, a private terrace, and beautiful seaside views.",
                            Rate = 425.0,
                            SqMeters = 200,
                            Occupancy = 5,
                            ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                            CreatedDate = new DateTime(2026, 3, 15),
                            UpdatedDate = new DateTime(2026, 3, 15)
                        },
            new Cottage
            {
                Id = 7,
                Name = "Pinewood Cottage",
                Details = "Escape to a tranquil woodland hideaway surrounded by towering pine trees, rustic interiors, and peaceful outdoor spaces perfect for relaxing.",
                Rate = 300.0,
                SqMeters = 160,
                Occupancy = 4,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
                CreatedDate = new DateTime(2026, 4, 1),
                UpdatedDate = new DateTime(2026, 4, 1)
            },
            new Cottage
            {
                Id = 8,
                Name = "Sunset Cottage",
                Details = "Enjoy unforgettable evenings in a stylish cottage with panoramic sunset views, comfortable living areas, and a spacious private balcony.",
                Rate = 475.0,
                SqMeters = 190,
                Occupancy = 5,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                CreatedDate = new DateTime(2026, 4, 15),
                UpdatedDate = new DateTime(2026, 4, 15)
            },
            new Cottage
            {
                Id = 9,
                Name = "Lakeside Cottage",
                Details = "Discover a peaceful lakeside retreat with scenic water views, cosy interiors, and a private outdoor seating area ideal for morning coffee.",
                Rate = 550.0,
                SqMeters = 230,
                Occupancy = 6,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
                CreatedDate = new DateTime(2026, 5, 1),
                UpdatedDate = new DateTime(2026, 5, 1)
            },
            new Cottage
            {
                Id = 10,
                Name = "Meadow Cottage",
                Details = "Relax in a welcoming countryside cottage surrounded by open meadows, colourful wildflowers, and picturesque walking paths.",
                Rate = 250.0,
                SqMeters = 140,
                Occupancy = 3,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
                CreatedDate = new DateTime(2026, 5, 15),
                UpdatedDate = new DateTime(2026, 5, 15)
            },
            new Cottage
            {
                Id = 11,
                Name = "Mountain View Cottage",
                Details = "Experience a memorable mountain escape with expansive windows, inviting living spaces, and breathtaking views of the surrounding peaks.",
                Rate = 625.0,
                SqMeters = 260,
                Occupancy = 7,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                CreatedDate = new DateTime(2026, 6, 1),
                UpdatedDate = new DateTime(2026, 6, 1)
            },
            new Cottage
            {
                Id = 12,
                Name = "Willow Cottage",
                Details = "Find comfort in this elegant cottage featuring warm wooden finishes, a beautifully landscaped garden, and peaceful spaces for the whole family.",
                Rate = 325.0,
                SqMeters = 170,
                Occupancy = 4,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
                CreatedDate = new DateTime(2026, 6, 15),
                UpdatedDate = new DateTime(2026, 6, 15)
            },
            new Cottage
            {
                Id = 13,
                Name = "Horizon Cottage",
                Details = "Enjoy a contemporary getaway with sophisticated furnishings, an open-plan layout, and sweeping views across the surrounding countryside.",
                Rate = 700.0,
                SqMeters = 300,
                Occupancy = 8,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                CreatedDate = new DateTime(2026, 7, 1),
                UpdatedDate = new DateTime(2026, 7, 1)
            },
            new Cottage
            {
                Id = 14,
                Name = "Rosewood Cottage",
                Details = "Step into a charming retreat with tasteful interiors, a private garden filled with seasonal flowers, and inviting spaces for a relaxing stay.",
                Rate = 375.0,
                SqMeters = 180,
                Occupancy = 5,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
                CreatedDate = new DateTime(2026, 7, 15),
                UpdatedDate = new DateTime(2026, 7, 15)
            },
            new Cottage
            {
                Id = 15,
                Name = "Evergreen Cottage",
                Details = "Reconnect with nature in a spacious woodland retreat featuring natural materials, comfortable bedrooms, and a secluded patio surrounded by greenery.",
                Rate = 450.0,
                SqMeters = 220,
                Occupancy = 6,
                ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
                CreatedDate = new DateTime(2026, 8, 1),
                UpdatedDate = new DateTime(2026, 8, 1)
            }


            );
        }
    }
}
