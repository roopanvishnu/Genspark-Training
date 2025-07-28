// Interfaces/IColorService.cs
using ChienVHShopAPI.Dtos;

namespace ChienVHShopAPI.Interfaces
{
    public interface IColorService
    {
        Task<List<ColorDto>> GetAllAsync();
        Task<ColorDto?> GetByIdAsync(int id);
        Task<ColorDto> CreateAsync(ColorDto dto);
        Task<bool> UpdateAsync(int id, ColorDto dto);
        Task<bool> DeleteAsync(int id);
    }
}