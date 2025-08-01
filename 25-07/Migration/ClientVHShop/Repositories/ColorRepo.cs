using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class ColorRepo : Repo<int, Color>
{
    public ColorRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<Color> Get(int id)
    {
        return await _context.Colors.FindAsync(id) ?? throw new Exception("Color not found");
    }

    public override async Task<ICollection<Color>> GetAll()
    {
        return await _context.Colors.ToListAsync() ?? throw new Exception("No Colors found");
    }
}