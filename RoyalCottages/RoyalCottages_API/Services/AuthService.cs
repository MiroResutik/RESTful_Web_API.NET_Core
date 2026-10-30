using RoyalCottages_API.Models.DTO;

namespace RoyalCottages_API.Services
{
    public class AuthService : IAuthService
    {
        // Endpoint to check if an email already exists
        public Task<bool> EmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        // Endpoint for user login
        public Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

        // Endpoint for user registration
        public Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
