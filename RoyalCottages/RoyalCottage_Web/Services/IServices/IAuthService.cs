using RoyalCottage_DTO;

namespace RoyalCottage_Web.Services.IServices
{
    public interface IAuthService
    {
        // Define a constant for the API endpoint used for authentication-related requests
        Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO);
        Task<T?> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO);

    }
}
