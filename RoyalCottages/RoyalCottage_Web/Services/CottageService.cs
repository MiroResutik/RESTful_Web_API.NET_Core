using RoyalCottage_DTO;
using RoyalCottage_Web.Extensions;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;

namespace RoyalCottage_Web.Services
{
    public class CottageService : BaseService, ICottageService
    {
        // Private field to hold the base URL for the Cottage API
        private readonly string _cottageUrl;

        // Constant string to define the API endpoint for cottages
        private const string APIEndpoint = $"/api/{SD.CurrentAPIVersion}/cottage";

        // Constructor to initialize the CottageService with dependencies
        public CottageService(IHttpClientFactory httpClient, IConfiguration configuration, ITokenProvider tokenProvider) : base(httpClient,tokenProvider)
        {
            
        }

        // Method to create a new cottage by sending a POST request to the API
        public Task<T?> CreateAsync<T>(CottageCreateDTO dto )
        {
            // Convert the CottageCreateDTO to MultipartFormDataContent for sending multipart/form-data requests
            var formData = dto.ToMultipartFormDataContent();

            // Send a POST request to the API endpoint with the provided DTO and token
            return SendAsync<T>(new ApiRequest
            {
                ApiType=SD.ApiType.POST,
                Data = formData,
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
            // Convert the CottageUpdateDTO to MultipartFormDataContent for sending multipart/form-data requests
            var formData = dto.ToMultipartFormDataContent();

            // Send a PUT request to the API endpoint with the provided DTO and token
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.PUT,
                Data = formData,
                Url = $"{APIEndpoint}/{dto.Id}"
            });
        }
    }
}
