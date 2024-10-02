
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]

public class UploadController : ControllerBase
{
    private readonly ApplicationContext _context;
    public UploadController(ApplicationContext context)
    {
        _context = context;
    }


    [HttpPost("uploadPicture")]
    public async Task<IActionResult> uploadPicture(IFormFile file)
    {
        if (file.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // You can then save the file to a database or a file system
            // For example:
            // _context.Images.Add(new Image { FilePath = filePath });
            // await _context.SaveChangesAsync();

            return Ok("Image uploaded successfully");
        }

        return BadRequest("No file selected");
    }
}