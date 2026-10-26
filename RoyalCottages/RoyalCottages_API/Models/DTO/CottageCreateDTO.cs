using System.ComponentModel.DataAnnotations;

namespace RoyalCottages_API.Models.DTO
{
    public class CottageCreateDTO
    {

        // DTO - Data Transfer Object - to transfer data between layers of the application
        [MaxLength(50)]
        [Required]
        public required string Name { get; set; }
        public string? Details { get; set; }
        public double Rate { get; set; }
        public int SqMeters { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }

    }
}
