using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;

namespace RoyalCottage_Web.Services
{
    public class CottageService : BaseService, ICottageService
    {
        // Private field to hold the base URL for the Cottage API
        private readonly string _cottageUrl;

        // Constant string to define the API endpoint for cottages
        private const string APIEndpoint = "/api/cottage";

        // Constructor to initialize the CottageService with HttpClient and configuration
        public CottageService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            
        }

        // Method to create a new cottage by sending a POST request to the API
        public Task<T?> CreateAsync<T>(CottageCreateDTO dto, string token)
        {
            // Send a POST request to the API endpoint with the provided DTO and token
            return SendAsync<T>(new ApiRequest
            {
                ApiType=SD.ApiType.POST,
                Data = dto,
                Url = APIEndpoint,
                Token = token
            });
        }

        // Method to delete a cottage by sending a DELETE request to the API
        public Task<T?> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{APIEndpoint}/{id}",
                Token = token
            });
        }

        // Method to retrieve a specific cottage by sending a GET request to the API
        public Task<T?> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{APIEndpoint}",
                Token = token
            });
        }

        // Method to retrieve a specific cottage by sending a GET request to the API
        public Task<T?> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Url = $"{APIEndpoint}/{id}",
                Token = token
            });
        }

        // Method to update a cottage by sending a PUT request to the API
        public Task<T?> UpdateAsync<T>(CottageUpdateDTO dto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = $"{APIEndpoint}/{dto.Id}",
                Token = token
            });
        }
    }
}
