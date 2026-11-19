using Microsoft.AspNetCore.Identity;

namespace RoyalCottages_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
