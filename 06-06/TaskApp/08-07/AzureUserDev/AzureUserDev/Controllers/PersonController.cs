using AzureUserDev.data;
using AzureUserDev.model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureUserDev.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly AppDbContext _context;

    public PersonController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/person
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetAll()
    {
        return await _context.Persons.ToListAsync();
    }

    // POST: api/person
    [HttpPost]
    public async Task<ActionResult<Person>> Create(Person person)
    {
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = person.Id }, person);
    }
}