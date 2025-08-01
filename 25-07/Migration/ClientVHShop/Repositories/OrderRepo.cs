using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class OrderRepo : Repo<int, Order>
{
    public OrderRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<Order> Get(int id)
    {
        return await _context.Orders.FindAsync(id) ?? throw new Exception("Order not found");
    }

    public override async Task<ICollection<Order>> GetAll()
    {
        return await _context.Orders.ToListAsync() ?? throw new Exception("No Orders found");
    }
}