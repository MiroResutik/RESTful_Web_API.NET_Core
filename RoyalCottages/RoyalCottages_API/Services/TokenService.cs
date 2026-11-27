using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoyalCottages_API.Services
{
    public class TokenService : ITokenService
    {

        // Inject the database context into the service
        private readonly ApplicationDbContext _db;

        // Inject the configuration into the service
        private readonly IConfiguration _configuration;

        // Inject the UserManager for managing users
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor for dependency injection
        public TokenService(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _configuration = configuration;
            _userManager = userManager;
        }

        // Generate a JWT token for the specified user
        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            // Get the secret key from the configuration
            var secret = _configuration.GetSection("JwtSettings")["Secret"];
            if (string.IsNullOrEmpty(secret)) throw new InvalidOperationException("JwtSecret not configured");
            var key = Encoding.ASCII.GetBytes(secret);

            // Get the roles of the user from the UserManager
            var roles = await _userManager.GetRolesAsync(user);

            // Create a token descriptor with claims, expiration, and signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Set the subject of the token with claims for user ID, email, and role
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                // Set the signing credentials using the secret key and HMAC SHA256 algorithm
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create a token handler to generate the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Create the token using the token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Return the serialized token as a string
            return tokenHandler.WriteToken(token);
        }
    }
}
