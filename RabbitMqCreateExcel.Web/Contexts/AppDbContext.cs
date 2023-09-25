using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RabbitMqCreateExcel.Web.Models;

namespace RabbitMqCreateExcel.Web.Contexts
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<UserFile> UserFiles { get; set; }

    }
}
