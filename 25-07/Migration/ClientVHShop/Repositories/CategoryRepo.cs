using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class CategoryRepo : Repo<int, Category>
{
    public CategoryRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<Category> Get(int id)
    {
        return await _context.Categories.FindAsync(id) ?? throw new Exception("Category not found");
    }

    public override async Task<ICollection<Category>> GetAll()
    {
        return await _context.Categories.ToListAsync() ?? throw new Exception("No categories found");
    }
}