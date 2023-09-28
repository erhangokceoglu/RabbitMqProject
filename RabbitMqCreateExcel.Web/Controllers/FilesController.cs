using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMqCreateExcel.Web.Contexts;
using RabbitMqCreateExcel.Web.Hubs;

namespace RabbitMqCreateExcel.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MyHub> _hubContext;

        public FilesController(AppDbContext context, IHubContext<MyHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
            await _hubContext.Clients.User(userfile.UserId).SendAsync("CompletedFile");
            return Ok();
        }
    }
}
