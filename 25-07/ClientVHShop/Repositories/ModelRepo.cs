using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class ModelRepo : Repo<int, Model>
{
    public ModelRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<Model> Get(int id)
    {
        return await _context.Models.FindAsync(id) ?? throw new Exception("Model not found");
    }

    public override async Task<ICollection<Model>> GetAll()
    {
        return await _context.Models.ToListAsync() ?? throw new Exception("No Models found");
    }
}