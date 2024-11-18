using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Controllers;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationContext _context;

    public UserController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<User>> GetUser(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return BadRequest("Name is required");
        }

        var user = await _context.User.FirstOrDefaultAsync(u => u.Name == name);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    public class updateRequest
    {
        public string? email { get; set; }
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
        public string? name { get; set; }
        public CLasses cLass { get; set; }
    }

    [HttpPut("{uuid}/change")]
    public async Task<IActionResult> UpdateUser(Guid uuid, [FromBody] updateRequest request)
    {
        Console.WriteLine($"Received Request: {request}");
        Console.WriteLine($"UUID: {uuid}");

        var foundUser = await _context.User.FirstOrDefaultAsync(u => u.Guid == uuid);
        if (foundUser == null)
        {
            return NotFound("User not found.");
        }

        if (!string.IsNullOrEmpty(request.oldPassword) && 
            !AuthService.VerifyPassword(request.oldPassword, foundUser.Password))
        {
            return BadRequest("Invalid old password.");
        }

        if (!string.IsNullOrEmpty(request.newPassword))
        {
            foundUser.Password = AuthService.HashPassword(request.newPassword);
        }

        if (!string.IsNullOrEmpty(request.email))
        {
            foundUser.Email = request.email;
        }

        if (!string.IsNullOrEmpty(request.name))
        {
            foundUser.Name = request.name;
        }

        foundUser.Class = request.cLass;

        _context.Entry(foundUser).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UserExistsAsync(uuid))
            {
                return NotFound("User not found during save.");
            }
            throw;
        }

        return NoContent();
    }


    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteUser(Guid uuid)
    {
        var user = await _context.User
            .Include(u => u.MapSpots)
            .FirstOrDefaultAsync(u => u.Guid == uuid);

        if (user == null)
        {
            return NotFound();
        }

        if (user.MapSpots != null && user.MapSpots.Any())
        {
            _context.MapSpot.RemoveRange(user.MapSpots);
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    
    [HttpPost("login")]
    public async Task<IActionResult> Login(User user)
    {
        var foundUser = await _context.User.FirstAsync(u => u.Email == user.Email);
        if (foundUser == null)
        {
            return BadRequest("Invalid email or password");
        }

        if (!AuthService.VerifyPassword(user.Password, foundUser.Password))
        {
            return BadRequest("Invalid email or password");
        }

        var tokenGenerator = new AuthService();
        var token = tokenGenerator.GenerateToken(foundUser);

        return Ok(new { token, foundUser });
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(User user)
    {
        if (_context.User.Any(u => u.Name == user.Name))
        {
            return Conflict();
        }

        var newUser = new User
        {
            Name = user.Name,
            Email = user.Email,
            Password = AuthService.HashPassword(user.Password),
            Class = user.Class
        };

        _context.User.Add(newUser);
        await _context.SaveChangesAsync();

        var tokenGenerator = new AuthService().GenerateToken;
        var token = tokenGenerator(newUser);

        return Ok(new { newUser, token });
    }

    private bool UserExists(Guid uuid)
    {
        return _context.User.Any(e => e.Guid == uuid);
    }

    private bool UserExistsWithId(int id)
    {
        return _context.User.Any(e => e.Id == id);
    }

    private async Task<bool> UserExistsAsync(Guid uuid)
    {
        return await _context.User.AnyAsync(u => u.Guid == uuid);
    }

}
