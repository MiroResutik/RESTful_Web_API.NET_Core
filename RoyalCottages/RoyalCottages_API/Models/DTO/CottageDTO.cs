using System.ComponentModel.DataAnnotations;

namespace RoyalCottages_API.Models.DTO
{
    public class CottageDTO
    {

        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Details { get; set; }
        public double Rate { get; set; }
        public int SqMeters { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }

    }
}
