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

        // Constructor to initialize the CottageService with dependencies
        public CottageService(IHttpClientFactory httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
            
        }

        // Method to create a new cottage by sending a POST request to the API
        public Task<T?> CreateAsync<T>(CottageCreateDTO dto )
        {
            // Send a POST request to the API endpoint with the provided DTO and token
            return SendAsync<T>(new ApiRequest
            {
                ApiType=SD.ApiType.POST,
                Data = dto,
                Url = APIEndpoint
            });
        }

        // Method to delete a cottage by sending a DELETE request to the API
        public Task<T?> DeleteAsync<T>(int id )
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{APIEndpoint}/{id}"
            });
        }

        // Method to retrieve a specific cottage by sending a GET request to the API
        public Task<T?> GetAllAsync<T>()
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{APIEndpoint}"
            });
        }

        // Method to retrieve a specific cottage by sending a GET request to the API
        public Task<T?> GetAsync<T>(int id)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{APIEndpoint}/{id}"
            });
        }

        // Method to update a cottage by sending a PUT request to the API
        public Task<T?> UpdateAsync<T>(CottageUpdateDTO dto)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = $"{APIEndpoint}/{dto.Id}"
            });
        }
    }
}
