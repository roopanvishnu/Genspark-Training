using ChienVHShopAPI.Contexts;
using ChienVHShopAPI.DTOs;
using ChienVHShopAPI.Interfaces;
using ChienVHShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ChienVHShopDbContext _context;

        public OrderService(ChienVHShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.OrderId)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    OrderDate = o.OrderDate,
                })
                .ToListAsync();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null) return null;

            return new OrderDto
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate,
            };
        }

        public async Task<OrderDto> CreateOrderAsync(OrderCreateDto dto)
        {
            var order = new Order
            {
                CustomerName = dto.CustomerName,
                OrderDate = dto.OrderDate,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
            };
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderUpdateDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            order.CustomerName = dto.CustomerName;
            order.OrderDate = dto.OrderDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
