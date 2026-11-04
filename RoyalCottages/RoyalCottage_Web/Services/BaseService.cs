using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;
using System.Text.Json;

namespace RoyalCottage_Web.Services
{
    public class BaseService : IBaseService
    {
        // Constructors
        // HttpClientFactory is used to create HttpClient instances for making HTTP requests
        public IHttpClientFactory _httpClient {  get; set; }

        // Optional - JsonSerializerOptions is used to configure the behavior of the JSON serializer and de-serializer
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        // Response model for API responses
        public ApiResponse<object> ResponseModel { get; set; }

        // Constructor for BaseService that takes an IHttpClientFactory as a parameter
        public BaseService(IHttpClientFactory httpClient) 
        {
            this.ResponseModel = new();
            _httpClient = httpClient;
        }

        // SendAsync is a generic method that sends an HTTP request to the specified API endpoint and returns the response as an object of type T
        public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                // Create an HttpClient instance using the IHttpClientFactory
                var client = _httpClient.CreateClient("RoyalCottageAPI");

                // Create an HttpRequestMessage with the specified URL and HTTP method based on the ApiType
                var message = new HttpRequestMessage
                {
                    RequestUri = new Uri(apiRequest.Url),
                    Method = GetHttpMethod(apiRequest.ApiType)
                };

                // Add the Authorization header with the Bearer token if it is provided in the ApiRequest
                if (apiRequest.Data != null)
                {
                    message.Content = JsonContent.Create(apiRequest.Data, options: JsonOptions);
                }
                // Send the HTTP request and get the response
                var apiResponse = await client.SendAsync(message);

                // Read the response content as JSON and deserialize it into an object of type T
                return await apiResponse.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return default;
            }
        }

        // GetHttpMethod is a private static method that returns the appropriate HttpMethod based on the specified ApiType
        private static HttpMethod GetHttpMethod(SD.ApiType apiType)
        {
            return apiType switch
            {
                SD.ApiType.POST => HttpMethod.Post,
                SD.ApiType.PUT => HttpMethod.Put,
                SD.ApiType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get, // Set GET as Default method
            };
        }
    }
}
