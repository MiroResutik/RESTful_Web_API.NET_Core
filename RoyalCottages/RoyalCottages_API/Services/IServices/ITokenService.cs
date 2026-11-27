using RoyalCottages_API.Models;

namespace RoyalCottages_API.Services.IServices
{
    public interface ITokenService
    {
        // Method to generate a JWT token for a given user
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);

    }
}
