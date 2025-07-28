using ChienVHShopAPI.DTOs;

namespace ChienVHShopAPI.Interfaces
{
    public interface INewsService
    {
        Task<List<NewsDto>> GetPagedNewsAsync(int pageNumber, int pageSize);
    }
}