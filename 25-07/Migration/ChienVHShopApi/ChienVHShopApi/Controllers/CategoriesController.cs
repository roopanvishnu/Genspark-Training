using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ChienVHShopAPI.Contexts;
using ChienVHShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ChienVHShopDbContext _db;

        public CategoriesController(ChienVHShopDbContext db)
        {
            _db = db;
        }

        // GET: api/categories
        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            var categories = _db.Categories.OrderBy(x => x.Name).ToList();
            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        public ActionResult<Category> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.Categories.Add(category);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
                return BadRequest("ID mismatch");

            if (!_db.Categories.Any(c => c.CategoryId == id))
                return NotFound();

            _db.Entry(category).State = EntityState.Modified;
            _db.SaveChanges();

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
                return NotFound();

            _db.Categories.Remove(category);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
