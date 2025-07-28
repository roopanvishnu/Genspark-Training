using ChienVHShopAPI.Contexts;
using Microsoft.AspNetCore.Mvc;
using ChienVHShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorsController : ControllerBase
    {
        private readonly ChienVHShopDbContext _context;

        public ColorsController(ChienVHShopDbContext context)
        {
            _context = context;
        }

        // GET: api/colors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Color>>> GetColors()
        {
            return await _context.Colors.ToListAsync();
        }

        // GET: api/colors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Color>> GetColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
                return NotFound();
            return color;
        }

        // POST: api/colors
        [HttpPost]
        public async Task<ActionResult<Color>> PostColor(Color color)
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetColor), new { id = color.ColorId }, color);
        }

        // PUT: api/colors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColor(int id, Color color)
        {
            if (id != color.ColorId)
                return BadRequest();

            _context.Entry(color).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Colors.Any(e => e.ColorId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/colors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
                return NotFound();

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
