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
    private readonly IWebHostEnvironment _environment;

    public UploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

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

    [HttpPost("multiupload")]
    public async Task<IActionResult> UploadFiles(IList<IFormFile> files)
    {
        // Define and setup the directory where your files will be saved.
        var root = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }

        foreach (var file in files)
        {
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);

            var filePath = Path.Combine(root, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        return Ok(new { message = "Files uploaded successfully." });
    }

}