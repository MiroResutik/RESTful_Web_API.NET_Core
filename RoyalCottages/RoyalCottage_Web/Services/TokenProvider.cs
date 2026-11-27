using Microsoft.AspNetCore.Authentication.Cookies;
using RoyalCottage_Web.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RoyalCottage_Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        // Inject IHttpContextAccessor to access the current HTTP context
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor to initialize the IHttpContextAccessor dependency
        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Clear the JWT token from the session
        public void ClearToken()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(SD.SessionToken);
        }

        // Create a ClaimsPrincipal from the JWT token
        public ClaimsPrincipal? CreatePrincipalFromJwtToken(string token)
        {
            // Check if the token is null or empty, return null if it is
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            try
            {
                // Create a JwtSecurityTokenHandler to read the JWT token from the response
                var handler = new JwtSecurityTokenHandler();

                // Read the JWT token and extract claims from it
                var jwt = handler.ReadJwtToken(token);

                // Create a ClaimsIdentity to hold the claims extracted from the JWT token
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                // Extract claims from the JWT token and add them to the ClaimsIdentity
                var emailClaim = jwt.Claims.FirstOrDefault(u => u.Type == "email");

                // Add the email claim to the identity if it exists
                if (emailClaim != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Name, emailClaim.Value));
                }

                // Extract the role claim from the JWT token and add it to the ClaimsIdentity
                var roleClaim = jwt.Claims.FirstOrDefault(u => u.Type == "role");

                // Add the role claim to the identity if it exists
                if (roleClaim != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                // Extract the name claim from the JWT token and add it to the ClaimsIdentity
                var nameClaim = jwt.Claims.FirstOrDefault(u => u.Type == "name");

                // Add the name claim to the identity if it exists
                if (nameClaim != null)
                {
                    identity.AddClaim(new Claim("FullName", nameClaim.Value));
                }

                // Create a ClaimsPrincipal with the identity and sign in the user using cookie authentication
                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        // Retrieve the JWT token from the session
        public string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(SD.SessionToken);
        }

        // Store the JWT token in the session
        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(SD.SessionToken, token);
        }
    }
}
