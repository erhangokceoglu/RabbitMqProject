using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMqCreateExcel.Web.Contexts;
using RabbitMqCreateExcel.Web.Enums;
using RabbitMqCreateExcel.Web.Models;
using RabbitMqCreateExcel.Web.Services;

namespace RabbitMqCreateExcel.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public ProductController(AppDbContext context, UserManager<IdentityUser> userManager, RabbitMqPublisher rabbitMqPublisher)
        {
            _context = context;
            _userManager = userManager;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var hasUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            UserFile userFile = new()
            {
                UserId = hasUser!.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating,
            };

            await _context.UserFiles.AddAsync(userFile);
            await _context.SaveChangesAsync();
            _rabbitMqPublisher.Publish(new CreateExcelMessage() { FileId = userFile.Id });
            TempData["StartCreationExcel"] = true;
            return RedirectToAction(nameof(Files));
        }

        public async Task<IActionResult> Files()
        {
            var hasUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            return View(await _context.UserFiles.Where(x => x.UserId == hasUser!.Id).OrderByDescending(x => x.Id).ToListAsync());
        }
    }
}
