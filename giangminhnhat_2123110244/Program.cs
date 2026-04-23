using Microsoft.EntityFrameworkCore;
using EBikeAPI.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Đăng ký SQL Server với chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Cho phép React gọi tới
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

// Enable serving static files from wwwroot (for uploaded images)
app.UseStaticFiles();

// Ensure uploads directory exists at startup
var env = app.Environment;
var uploadPath = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads", "products");
if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

app.UseAuthorization();

app.MapControllers();

app.Run();
