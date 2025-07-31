using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class ProductRepo : Repo<int, Product>
{
    public ProductRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<Product> Get(int id)
    {
        return await _context.Products.FindAsync(id) ?? throw new Exception("Product not found");
    }

    public override async Task<ICollection<Product>> GetAll()
    {
        return await _context.Products.ToListAsync() ?? throw new Exception("No Products found");
    }
}