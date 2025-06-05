using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public class Sample2Controller : ControllerBase
    {
        [HttpGet]
        // [Route("/api/[controller]")]
        public IResult GetGreetings()
        {
            return Results.Ok("Hello World! - 2");
        }

        [HttpPost]
        public ActionResult PostGreetings(string greeting)
        {
            Console.WriteLine(greeting);
            return NotFound("404");
        }
    }
}