using System.ComponentModel.DataAnnotations;

namespace RoyalCottage_DTO
{
    public class CottageAmenitiesUpdateDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int CottageId { get; set; }
    }
}
