
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnumController : ControllerBase
{
    private readonly ApplicationContext _context;

    public EnumController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet("getSpotTypes")]
    public ActionResult<List<Enum>> Get()
    {
        var enums = Enum.GetValues<SpotType>();
        return Ok(enums);
    }

    [HttpGet("getClasses")]
    public ActionResult<List<Enum>> GetClasses()
    {
        var enums = Enum.GetValues<CLasses>();
        return Ok(enums);
    }
}