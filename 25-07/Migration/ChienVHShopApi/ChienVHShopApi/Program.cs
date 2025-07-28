

using ChienVHShopAPI.Contexts;
using ChienVHShopApi.Interfaces;
using ChienVHShopAPI.Interfaces;
using ChienVHShopApi.Services;
using ChienVHShopAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ChienVHShop API", Version = "v1" });
});
builder.Services.AddScoped<IProductService, ProductService>();


// PostgreSQL configuration
builder.Services.AddDbContext<ChienVHShopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper (optional but useful for DTOs)

// Dependency Injection (Add all service interfaces and implementations)
builder.Services.AddScoped<IContactUsService, ContactUsService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Recommended if using HTTPS
app.UseAuthorization();

app.MapControllers();

app.Run();
