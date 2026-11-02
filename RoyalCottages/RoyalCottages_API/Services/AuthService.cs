using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoyalCottages_API.Services
{
    // Service for handling authentication-related operations
    public class AuthService : IAuthService
    {
        // Inject the database context into the service
        private readonly ApplicationDbContext _db;
        // Inject the configuration into the service
        private readonly IConfiguration _configuration;
        // Inject the AutoMapper into the service
        private readonly IMapper _mapper;

        // Constructor for dependency injection
        public AuthService(ApplicationDbContext db, IConfiguration configuration, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _mapper = mapper;
        }

        // Endpoint to check if an email already exists
        public async Task<bool> EmailExistsAsync(string email)
        {
            // Check if the email exists in the database (case-insensitive)
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Endpoint for user login
        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            // Validate the login request
            try
            {
                // Find the user in the database by email (case-insensitive)
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower());

                // Check if the user object is null or if the password does not match
                if (user == null || user.Password != loginRequestDTO.Password)
                {
                    return null;
                }

                // Generate a JWT token for the authenticated user
                var token = GenerateJwtToken(user);


                // Create a login response DTO
                return new LoginResponseDTO
                {
                    UserDTO = _mapper.Map<UserDTO>(user),
                    Token = token
                };
               
            }
            catch (Exception ex)
            {
                // Log the exception and throw a new exception with a user-friendly message
                throw new InvalidOperationException($"Error occurred during user login: {ex.Message}");
            }
        }

        // Endpoint for user registration
        public async Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                // Check if the email already exists in the database
                if (await EmailExistsAsync(registrationRequestDTO.Email))
                {
                    throw new InvalidOperationException($"User with email {registrationRequestDTO.Email} already exists");
                }
                // Create a new user object from the registration request DTO
                User user = new()
                {
                    Email = registrationRequestDTO.Email,
                    Name = registrationRequestDTO.Name,
                    Password = registrationRequestDTO.Password,
                    Role = string.IsNullOrEmpty(registrationRequestDTO.Role) ? "Customer" : registrationRequestDTO.Role,
                    CreatedDate = DateTime.UtcNow,
                };

                // Add the new user to the database and save changes
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                // Map the user object to a UserDTO and return it
                return _mapper.Map<UserDTO>(user);

            }
            catch (Exception ex)
            {
                // Log the exception and throw a new exception with a user-friendly message
                throw new InvalidOperationException($"Error occurred while registering user: {ex.Message}");
            }
        }

        // Generate a JWT token for the authenticated user
        private string GenerateJwtToken(User user)
        {
            // Get the secret key from the configuration
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]);

            // Create a token descriptor with claims, expiration, and signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Set the subject of the token with claims for user ID, email, and role
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role)
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
