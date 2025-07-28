using ChienVHShopAPI.DTOs;

namespace ChienVHShopAPI.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(OrderCreateDto dto);
        Task<bool> UpdateOrderAsync(int id, OrderUpdateDto dto);
        Task<bool> DeleteOrderAsync(int id);
    }
}
