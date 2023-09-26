using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMqCreateExcel.Web.Contexts;

namespace RabbitMqCreateExcel.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            if (file is not { Length: > 0 }) return BadRequest();
            var userfile = await _context.UserFiles.FirstAsync(x => x.Id == fileId);
            var filePath = userfile.FileName + Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);
            using FileStream stream = new(path, FileMode.Create);
            await file.CopyToAsync(stream);
            userfile.CreatedDate = DateTime.Now;
            userfile.FilePath = filePath;
            userfile.FileStatus = Enums.FileStatus.Completed;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
