using System.ComponentModel.DataAnnotations;

namespace RoyalCottages_API.Models.DTO
{
    public class CottageAmenitiesCreateDTO
    {
        [Required]
        public int CottageId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }

    }
}
