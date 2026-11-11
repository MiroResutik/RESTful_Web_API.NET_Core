using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;

namespace RoyalCottage_Web.Services
{
    // AuthService class that implements the IAuthService interface for handling authentication-related operations
    public class AuthService : BaseService, IAuthService
    {
        // Define a constant for the API endpoint used for authentication-related requests
        private const string APIEndpoint = "/api/auth";

        // Constructor to initialize the AuthService with an IHttpClientFactory and IConfiguration
        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {

        }

        // Implement the LoginAsync method to handle user login requests
        public Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            // Send a POST request to the API endpoint with the provided DTO and token
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = APIEndpoint+"/login",
            });
        }

        // Implement the RegisterAsync method to handle user registration requests
        public Task<T?> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO)
        {
            // Send a POST request to the API endpoint with the provided DTO and token
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDTO,
                Url = APIEndpoint+"/register",
            });
        }
    }
}
