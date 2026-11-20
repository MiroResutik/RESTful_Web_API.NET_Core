using System.ComponentModel.DataAnnotations;

namespace RoyalCottage_DTO
{
    public class UserDTO
    {
        // Id changed from int to string when using ApplicationUser to register  
        public string Id { get; set; }

        public string Email { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Role { get; set; } = default!;
    }
}
