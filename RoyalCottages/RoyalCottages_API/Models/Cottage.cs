using System.ComponentModel.DataAnnotations;

namespace RoyalCottages_API.Models
{
    // Model for the Cottage 
    public class Cottage
    {
        // Define Cottage Table 
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Details { get; set; }
        public double Rate { get; set; }
        public int SqMeters { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        // Navigation property
        // In each cottage there can be multiple amenities associated with that cottage
        public ICollection<CottageAmenities>? Amenities { get; set; }
    }
}
