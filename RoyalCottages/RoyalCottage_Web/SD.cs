namespace RoyalCottage_Web
{
    public static class SD
    {
        // ApiType is an enumeration that defines the different types of HTTP methods that can be used in API requests
        public enum ApiType
        {
            GET, POST, PUT, DELETE
        }

        // SessionToken is a constant string that represents the key used to store the JWT token in the session
        public const string SessionToken = "JWTToken";

        // CurrentAPIVersion is a constant string that represents the current version of the API being used in the application
        public const string CurrentAPIVersion = "v2";

        // APIBaseUrl is a static property that holds the base URL for the API, which can be set and accessed throughout the application
        public static string APIBaseUrl {  get;  set; }

        // GetImageUrl is a static method that takes an image URL as input and returns the full URL for the image, handling cases where the image URL is null or empty
        public static string GetImageUrl(string? imageUrl)
        {
            // If the image URL is null or empty, return a placeholder image URL
            if (string.IsNullOrEmpty(imageUrl))
            {
                return "/images/placeholder_cottage.png";
            }
            // If the image URL starts with "https://", return it as is (it's already a full URL)
            if (imageUrl.StartsWith("https://"))
            {
                return imageUrl;
            }
            return $"{APIBaseUrl}{imageUrl}";
        }
    }
}
