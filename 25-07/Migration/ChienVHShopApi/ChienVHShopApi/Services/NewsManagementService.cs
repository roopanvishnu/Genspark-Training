using ChienVHShopAPI.DTOs;
using ChienVHShopAPI.Interfaces;
using ChienVHShopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ChienVHShopAPI.Contexts;
using OfficeOpenXml;
using OfficeOpenXml.Core.ExcelPackage; // Add EPPlus via NuGet for Excel export

namespace ChienVHShopAPI.Services
{
    public class NewsManagementService : INewsManagementService
    {
        private readonly ChienVHShopDbContext _context;

        public NewsManagementService(ChienVHShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<NewsAdminDto>> GetAllNewsAsync()
        {
            return await _context.News
                .Include(n => n.User)
                .Select(n => new NewsAdminDto
                {
                    NewsId = n.NewsId,
                    UserId = n.UserId,
                    Title = n.Title,
                    ShortDescription = n.ShortDescription,
                    Image = n.Image,
                    Content = n.Content,
                    CreatedDate = n.CreatedDate,
                    Username = n.User.Username
                })
                .ToListAsync();
        }

        public async Task<NewsAdminDto?> GetNewsByIdAsync(int id)
        {
            var n = await _context.News.Include(x => x.User).FirstOrDefaultAsync(x => x.NewsId == id);
            if (n == null) return null;

            return new NewsAdminDto
            {
                NewsId = n.NewsId,
                UserId = n.UserId,
                Title = n.Title,
                ShortDescription = n.ShortDescription,
                Image = n.Image,
                Content = n.Content,
                CreatedDate = n.CreatedDate,
                Username = n.User.Username
            };
        }

        public async Task<NewsAdminDto> CreateNewsAsync(NewsAdminDto dto)
        {
            var news = new News
            {
                UserId = dto.UserId,
                Title = dto.Title,
                ShortDescription = dto.ShortDescription,
                Image = dto.Image,
                Content = dto.Content,
                CreatedDate = dto.CreatedDate ?? DateTime.UtcNow,
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            dto.NewsId = news.NewsId;
            return dto;
        }

        public async Task<bool> UpdateNewsAsync(int id, NewsAdminDto dto)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return false;

            news.UserId = dto.UserId;
            news.Title = dto.Title;
            news.ShortDescription = dto.ShortDescription;
            news.Image = dto.Image;
            news.Content = dto.Content;
            news.CreatedDate = dto.CreatedDate ?? news.CreatedDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return false;

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> ExportNewsToCsvAsync()
        {
            var newsList = await _context.News.OrderBy(n => n.NewsId).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("\"NewsId\",\"Title\",\"ShortDescription\",\"CreatedDate\",\"Status\"");

            foreach (var n in newsList)
            {
                sb.AppendLine($"\"{n.NewsId}\",\"{n.Title}\",\"{n.ShortDescription}\",\"{n.CreatedDate}\",\"{n.Status}\"");
            }

            return sb.ToString();
        }

        
    }
}
