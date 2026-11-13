using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RoyalCottage_Web.Services
{
    public class BaseService : IBaseService
    {
        // Constructors
        // HttpClientFactory is used to create HttpClient instances for making HTTP requests
        public IHttpClientFactory _httpClient {  get; set; }

        // IHttpContextAccessor is used to access the current HTTP context, which allows retrieving information about the current request and session
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Optional - JsonSerializerOptions is used to configure the behavior of the JSON serializer and de-serializer
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        // Response model for API responses
        public ApiResponse<object> ResponseModel { get; set; }

        // Constructor to initialize the BaseService with dependencies
        public BaseService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor) 
        {
            this.ResponseModel = new();
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
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
                    RequestUri = new Uri(apiRequest.Url, uriKind:UriKind.Relative),
                    Method = GetHttpMethod(apiRequest.ApiType)
                };

                // Retrieve the Bearer token from the session and add it to the Authorization header if it exists
                var token = _httpContextAccessor.HttpContext?.Session?.GetString(SD.SessionToken);

                // Add the Authorization header with the Bearer token if it is provided in the ApiRequest
                if (!string.IsNullOrEmpty(token))
                {
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

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
