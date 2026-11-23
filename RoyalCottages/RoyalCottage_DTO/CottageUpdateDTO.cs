using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RoyalCottage_DTO
{
    public class CottageUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        // DTO - Data Transfer Object - to transfer data between layers of the application
        [MaxLength(50)]
        [Required]
        public required string Name { get; set; }
        public string? Details { get; set; }
        public double Rate { get; set; }
        public int SqMeters { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        // Property to hold the uploaded image file
        public IFormFile? Image { get; set; }

    }
}
