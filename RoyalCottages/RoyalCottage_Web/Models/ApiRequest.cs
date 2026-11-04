using static RoyalCottage_Web.SD;

namespace RoyalCottage_Web.Models
{
    // ApiRequest is a model class that represents an API request with properties for the API type, URL, data, and token
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET; // Default if the api request not set
        public string? Url { get; set; }
        public object? Data { get; set; }
        public string? Token { get; set; }
    }
}
