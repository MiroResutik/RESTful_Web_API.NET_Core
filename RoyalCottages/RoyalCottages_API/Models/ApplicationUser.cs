using Microsoft.AspNetCore.Identity;

namespace RoyalCottages_API.Models
{
    // ApplicationUser class inherits from IdentityUser to represent a user in the application
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
