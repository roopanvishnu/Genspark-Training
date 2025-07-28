using ChienVHShopApi.Dtos;

namespace ChienVHShopApi.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(int? categoryId);
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}
