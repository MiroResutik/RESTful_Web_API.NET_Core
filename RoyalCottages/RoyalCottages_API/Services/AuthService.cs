using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Models.DTO;

namespace RoyalCottages_API.Services
{
    public class AuthService : IAuthService
    {
        // Inject the database context into the service
        private readonly ApplicationDbContext _db;

        // Inject the AutoMapper into the service
        private readonly IMapper _mapper;

        // Constructor for dependency injection
        public AuthService(ApplicationDbContext db, IConfiguration configuration, IMapper mapper)
        {
            _db = db;
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
            throw new NotImplementedException();
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
    }
}
