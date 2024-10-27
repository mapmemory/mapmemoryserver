using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]

public class MapSpotController : ControllerBase
{
    private readonly ApplicationContext _context;

    public MapSpotController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet("getSpots")]
    public async Task<ActionResult<List<MapSpot>>> GetSpots()
    {
        var spots = await _context.MapSpot
        .Include(m => m.User)
        .ToListAsync();

        var result = spots
        .Select(m => new {m.Id, m.Guid, m.Description, m.Type, m.User!.Name, m.Latitude, m.Longitude, m.picture});

        return Ok(result);
    }

    [HttpGet("getRandomSpotGuid")]
    public async Task<ActionResult<Guid>> GetRandomSpotGuid()
    {
        int totalSpots = await _context.MapSpot.CountAsync();
        if (totalSpots == 0)
        {
            return NotFound("No spots found.");
        }

        // Generate a random index based on the total number of spots
        int randomIndex = new Random().Next(totalSpots);

        // Retrieve the Guid of the spot at the random index
        var randomSpotGuid = await _context.MapSpot
            .Skip(randomIndex)
            .Select(s => s.Guid)
            .FirstOrDefaultAsync();

        return Ok(randomSpotGuid);
    }



    [HttpGet("{guid}")]
    public async Task<ActionResult> GetSpotByGuid(Guid guid)
    {
        var spot = await _context.MapSpot
            .Include(m => m.User)
            .Where(m => m.Guid == guid)
            .Select(m => new
            {
                m.Id,
                m.Guid,
                m.Description,
                m.Type,
                m.User!.Name,
                m.Latitude,
                m.Longitude,
                m.picture
            })
            .FirstOrDefaultAsync();

        if (spot == null)
        {
            return NotFound($"Spot with GUID {guid} not found.");
        }

        return Ok(spot);
    }

    [HttpPost("addSpot")]
    public async Task<ActionResult<List<MapSpot>>> AddSpot([FromBody] MapSpot spot)
    {
        _context.MapSpot.Add(spot);
        await _context.SaveChangesAsync();
        return Ok(spot);
    }

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteSpot(Guid uuid)
    {
        var spot = await _context.MapSpot.FindAsync(uuid);
        if (spot == null)
        {
            return NotFound();
        }
        _context.MapSpot.Remove(spot);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{uuid}")]
    public async Task<IActionResult> UpdateSpot(Guid uuid, [FromBody] MapSpot spot)
    {
        var foundSpot = await _context.MapSpot.FindAsync(uuid);
        if (foundSpot == null)
        {
            return NotFound();
        }
        foundSpot.Description = spot.Description;
        foundSpot.Type = spot.Type;
        foundSpot.Latitude = spot.Latitude;
        foundSpot.Longitude = spot.Longitude;
        foundSpot.picture = spot.picture;
        _context.Entry(foundSpot).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}