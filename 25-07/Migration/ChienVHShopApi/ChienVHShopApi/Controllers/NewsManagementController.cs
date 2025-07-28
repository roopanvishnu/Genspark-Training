using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChienVHShopAPI.Models;
using System.Text;
using System.Threading.Tasks;
using ChienVHShopAPI.Contexts;

namespace ChienVHShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsManagementController : ControllerBase
    {
        private readonly ChienVHShopDbContext _context;

        public NewsManagementController(ChienVHShopDbContext context)
        {
            _context = context;
        }

        // GET: api/NewsManagement
        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            var newsList = await _context.News.Include(n => n.User).ToListAsync();
            return Ok(newsList);
        }

        // GET: api/NewsManagement/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();
            return Ok(news);
        }

        // POST: api/NewsManagement
        [HttpPost]
        public async Task<IActionResult> CreateNews([FromBody] News news)
        {
            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNews), new { id = news.NewsId }, news);
        }

        // PUT: api/NewsManagement/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] News news)
        {
            if (id != news.NewsId)
                return BadRequest();

            _context.Entry(news).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/NewsManagement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/NewsManagement/export/csv
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportToCSV()
        {
            var newsList = await _context.News.OrderBy(n => n.NewsId).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("\"NewsId\",\"Title\",\"ShortDescription\",\"CreatedDate\",\"Status\"");

            foreach (var n in newsList)
            {
                sb.AppendLine($"\"{n.NewsId}\",\"{n.Title}\",\"{n.ShortDescription}\",\"{n.CreatedDate}\",\"{n.Status}\"");
            }

            var content = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"NewsListing_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(content, "text/csv", fileName);
        }

        // GET: api/NewsManagement/export/excel
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var newsList = await _context.News.OrderBy(n => n.NewsId).ToListAsync();
            var sb = new StringBuilder();

            sb.AppendLine("<table border='1'><tr><th>NewsId</th><th>Title</th><th>ShortDescription</th><th>CreatedDate</th><th>Status</th></tr>");
            foreach (var n in newsList)
            {
                sb.AppendLine($"<tr><td>{n.NewsId}</td><td>{n.Title}</td><td>{n.ShortDescription}</td><td>{n.CreatedDate}</td><td>{n.Status}</td></tr>");
            }
            sb.AppendLine("</table>");

            var content = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"NewsListing_{DateTime.Now:yyyyMMddHHmmss}.xls";
            return File(content, "application/vnd.ms-excel", fileName);
        }
    }
}
