using RoyalCottage_DTO;

namespace RoyalCottage_Web.Services.IServices
{
    public interface ICottageService
    {
        Task<T?> GetAllAsync<T>(string token);

        Task<T?> GetAsync<T>(int id,string token);

        Task<T?> CreateAsync<T>(CottageCreateDTO dto, string token);

        Task<T?> UpdateAsync<T>(CottageUpdateDTO dto, string token);

        Task<T?> DeleteAsync<T>(int id, string token);

    }
}
