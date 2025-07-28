// Services/ColorService.cs
using ChienVHShopAPI.Contexts;
using ChienVHShopAPI.Dtos;
using ChienVHShopAPI.Interfaces;
using ChienVHShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopAPI.Services
{
    public class ColorService : IColorService
    {
        private readonly ChienVHShopDbContext _context;

        public ColorService(ChienVHShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<ColorDto>> GetAllAsync()
        {
            return await _context.Colors
                .Select(c => new ColorDto { ColorId = c.ColorId, Name = c.ColorName })
                .ToListAsync();
        }

        public async Task<ColorDto?> GetByIdAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return null;

            return new ColorDto { ColorId = color.ColorId, Name = color.ColorName };
        }

        public async Task<ColorDto> CreateAsync(ColorDto dto)
        {
            var color = new Color { ColorName = dto.Name };
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            dto.ColorId = color.ColorId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, ColorDto dto)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return false;

            color.ColorName = dto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return false;

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}