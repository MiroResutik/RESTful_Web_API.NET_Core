using System.Security.Claims;

namespace RoyalCottage_Web.Services.IServices
{
    public interface ITokenProvider
    {
        // Set the JWT token in the provider
        void SetToken(string token);
        // Get the JWT token from the provider
        string? GetToken();
        // Clear the JWT token from the provider
        void ClearToken();
        // Create a ClaimsPrincipal from the JWT token
        ClaimsPrincipal? CreatePrincipalFromJwtToken(string token);
    }
}
