using RoyalCottage_DTO;

namespace RoyalCottages_API.Services.IServices
{
    public interface IAuthService
    {
        // Endpoint for user registration
        Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);

        // Endpoint for user login
        Task<TokenDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);

        // Endpoint to check if an email already exists
        Task<bool> EmailExistsAsync(string email);

    }
}
