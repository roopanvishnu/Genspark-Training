using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class OrderDetailRepo : Repo<int, OrderDetail>
{
    public OrderDetailRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<OrderDetail> Get(int id)
    {
        return await _context.OrderDetails.FindAsync(id) ?? throw new Exception("OrderDetail not found");
    }

    public override async Task<ICollection<OrderDetail>> GetAll()
    {
        return await _context.OrderDetails.ToListAsync() ?? throw new Exception("No OrderDetails found");
    }
}