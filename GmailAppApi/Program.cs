using GmailAppApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// Cấu hình để mặc định load index.html từ thư mục Login-form
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "Login-form")),
    RequestPath = "",
    DefaultFileNames = new List<string> { "index.html" }
});

// Cho phép load toàn bộ file tĩnh mặc định từ wwwroot
app.UseStaticFiles(); // wwwroot/css, wwwroot/js, wwwroot/images

// Cho phép load file tĩnh trong thư mục Login-form
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "Login-form")),
    RequestPath = ""
});

// 👉 Thêm cấu hình để phục vụ thư mục wwwroot/html
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "html")),
    RequestPath = "/html"
});

app.MapControllers();
app.Run();
