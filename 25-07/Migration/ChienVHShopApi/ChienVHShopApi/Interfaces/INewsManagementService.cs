using ChienVHShopAPI.DTOs;

namespace ChienVHShopAPI.Interfaces
{
    public interface INewsManagementService
    {
        Task<List<NewsAdminDto>> GetAllNewsAsync();
        Task<NewsAdminDto?> GetNewsByIdAsync(int id);
        Task<NewsAdminDto> CreateNewsAsync(NewsAdminDto dto);
        Task<bool> UpdateNewsAsync(int id, NewsAdminDto dto);
        Task<bool> DeleteNewsAsync(int id);

        Task<string> ExportNewsToCsvAsync();
    }
}
