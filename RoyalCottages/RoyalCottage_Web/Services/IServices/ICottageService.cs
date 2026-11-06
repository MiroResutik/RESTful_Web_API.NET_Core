using RoyalCottage_DTO;

namespace RoyalCottage_Web.Services.IServices
{
    public interface ICottageService
    {
        // Method to create a new cottage by sending a POST request to the API
        Task<T?> GetAllAsync<T>(string token);
        // Method to retrieve a specific cottage by sending a GET request to the API
        Task<T?> GetAsync<T>(int id,string token);
        // Method to update a cottage by sending a PUT request to the API
        Task<T?> CreateAsync<T>(CottageCreateDTO dto, string token);
        // Method to update a cottage by sending a PUT request to the API
        Task<T?> UpdateAsync<T>(CottageUpdateDTO dto, string token);
        // Method to delete a cottage by sending a DELETE request to the API
        Task<T?> DeleteAsync<T>(int id, string token);

    }
}
