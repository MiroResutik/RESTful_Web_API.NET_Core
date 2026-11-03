using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoyalCottages_API.Models
{
    public class CottageAmenities
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [Required]
        [ForeignKey(nameof(Cottage))] // Table that has the foreign key relation
        public int CottageId { get; set; }
        // To populate the navigation property when the CottageAmenities object is loaded
        public Cottage? Cottage { get; set; }

    }
}
