using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace upload.Controllers;

[Route("api/[controller]")]
[ApiController]
[RequestSizeLimit(1_000_000)]
[RequestFormLimits(MultipartBodyLengthLimit = 1_000_000)]
public class UploadController : ControllerBase
{
    // POST api/upload
    [HttpPost]
    public async Task<IActionResult> Post([FromForm] Upload uploadFile)
    {
        IFormFile? file = uploadFile.File;

        if (file == null || file.Length == 0)
            return BadRequest("No file selected");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) ||
            (extension != ".pdf" && extension != ".png" && extension != ".jpg" && extension != ".jpeg"))
            return BadRequest("Invalid file type");

        // Generate a new file name with a GUID
        var newFileName = Path.GetRandomFileName() + extension;

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var filePath = Path.Combine(folderPath, newFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = Url.Content($"~/uploads/{newFileName}");
        return Ok(new { url = fileUrl });
    }
}