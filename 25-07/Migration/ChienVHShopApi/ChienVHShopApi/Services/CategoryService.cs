using ChienVHShopApi.DTOs;
using ChienVHShopApi.Interfaces;
using ChienVHShopAPI.Contexts;
using ChienVHShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ChienVHShopDbContext _context;

        public CategoryService(ChienVHShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            categoryDto.CategoryId = category.CategoryId;
            return categoryDto;
        }

        public async Task<bool> UpdateAsync(int id, CategoryDto categoryDto)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            existing.Name = categoryDto.Name;
            _context.Categories.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
