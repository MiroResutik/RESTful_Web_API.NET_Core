using RoyalCottages_API.Models.DTO;

namespace RoyalCottages_API.Services
{
    public interface IAuthService
    {
        // Endpoint for user registration
        Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);

        // Endpoint for user login
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);

        // Endpoint to check if an email already exists
        Task<bool> EmailExistsAsync(string email);

    }
}
