
using ChienVHShopAPI.Contexts;
using ChienVHShopApi.Dtos;
using ChienVHShopApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopApi.Services
{
    public class ProductService : IProductService
    {
        private readonly ChienVHShopDbContext _context;

        public ProductService(ChienVHShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(int? categoryId)
        {
            var query = _context.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            var products = await query
                .OrderByDescending(p => p.ProductId)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.ProductName,
                    CategoryId = p.CategoryId,
                    Price = p.Price,
                    Image = p.Image
                })
                .ToListAsync();

            return products;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.ProductName,
                    CategoryId = p.CategoryId,
                    Price = p.Price,
                    Image = p.Image
                })
                .FirstOrDefaultAsync();

            return product;
        }
    }
}
