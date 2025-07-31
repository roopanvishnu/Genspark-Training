using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class NewsRepo : Repo<int, News>
{
    public NewsRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<News> Get(int id)
    {
        return await _context.News.Include(n => n.User).FirstOrDefaultAsync(n => n.NewsId == id) ?? throw new Exception("News not found");
    }

    public override async Task<ICollection<News>> GetAll()
    {
        return await _context.News.Include(n => n.User).ToListAsync() ?? throw new Exception("No News found");
    }
}