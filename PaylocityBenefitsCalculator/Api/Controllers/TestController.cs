using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    private readonly BenefitsDbContext _context;
    public TestController(BenefitsDbContext context)
    {
        _context = context;
    }

    [SwaggerOperation(Summary = "Seed the database, for integration testing...")]
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        DataSeeder.Seed(_context);
        return Ok();
    }
}