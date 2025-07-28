using ChienVHShopAPI.Contexts;
using ChienVHShopAPI.DTOs;
using ChienVHShopAPI.Interfaces;
using ChienVHShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopAPI.Services
{
    public class NewsService : INewsService
    {
        private readonly ChienVHShopDbContext _context;

        public NewsService(ChienVHShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<NewsDto>> GetPagedNewsAsync(int pageNumber, int pageSize)
        {
            return await _context.News
                .OrderByDescending(n => n.NewsId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NewsDto
                {
                    NewsId = n.NewsId,
                    Title = n.Title,
                    Description = n.ShortDescription,
                    Image = n.Image,
                    CreatedDate = n.CreatedDate
                })
                .ToListAsync();
        }
    }
}