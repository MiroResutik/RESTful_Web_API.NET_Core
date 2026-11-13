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
    }
}
