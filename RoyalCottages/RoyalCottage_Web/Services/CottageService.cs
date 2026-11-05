using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;

namespace RoyalCottage_Web.Services
{
    public class CottageService : BaseService, ICottageService
    {

        private readonly string _cottageUrl;

        private const string APIEndpoint = "/api/cottage";

        public CottageService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            
            _cottageUrl = configuration.GetValue<string>("ServiceUrl:CottageAPI");
        }

        public Task<T?> CreateAsync<T>(CottageCreateDTO dto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType=SD.ApiType.POST,
                Data = dto,
                Url = $"{_cottageUrl}{APIEndpoint}",
                Token = token
            });
        }

        public Task<T?> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{_cottageUrl}{APIEndpoint}/{id}",
                Token = token
            });
        }

        public Task<T?> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"{_cottageUrl}{APIEndpoint}",
                Token = token
            });
        }

        public Task<T?> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Url = $"{_cottageUrl}{APIEndpoint}/{id}",
                Token = token
            });
        }

        public Task<T?> UpdateAsync<T>(CottageUpdateDTO dto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = $"{_cottageUrl}{APIEndpoint}/{dto.Id}",
                Token = token
            });
        }
    }
}
