using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqCreateExcel.Web.Contexts;
using RabbitMqCreateExcel.Web.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("RabbitMqApp"), options =>
    {
        options.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext))!.GetName().Name);
    });
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddSingleton(x => new ConnectionFactory()
{
    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMq")!
    ),
    DispatchConsumersAsync = true
});

builder.Services.AddSingleton<RabbitMqClientService>();
builder.Services.AddSingleton<RabbitMqPublisher>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
    db?.Database.Migrate();

    if (!db!.Users.Any())
    {
        userManager!.CreateAsync(new IdentityUser { UserName = "johndoe", Email = "johndoe@hotmail.com" }, password: "Ankara1.").Wait();
        userManager.CreateAsync(new IdentityUser { UserName = "ericdoe", Email = "ericdoe@hotmail.com" }, password: "Ankara1.").Wait();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
