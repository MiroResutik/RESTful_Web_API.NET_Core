using RoyalCottage_DTO;
using RoyalCottage_Web.Models;

namespace RoyalCottage_Web.Services.IServices
{
    public interface IBaseService
    {
        // HttpClientFactory is used to create HttpClient instances for making HTTP requests
        ApiResponse<object> ResponseModel { get; set; }

        // SendAsync is a generic method that sends an HTTP request to the specified API endpoint and returns the response as an object of type T
        Task<T?> SendAsync<T>(ApiRequest apiRequest);
    }
}
