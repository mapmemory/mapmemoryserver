
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

            return Ok(fileName);
        }

        return BadRequest("No file selected");
    }

    [HttpPost("uploadPictures")]
    public async Task<IActionResult> uploadPictures(IFormFile[] files)
    {
        if (files.Length == 0)
        {
            return BadRequest("No files selected");
        }

        var fileGuid = Guid.NewGuid().ToString();

        for (int i = 0; i < files.Length; i++)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileGuid +"-"+ (i+1) + Path.GetExtension(files[i].FileName));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await files[i].CopyToAsync(stream);
            }
        }

        return Ok(new { fileGuid });
    }

    
}