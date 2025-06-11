using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TaskManagement.API.Context;
using TaskManagement.API.Filters;
using TaskManagement.API.Hubs;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Implementations;
using TaskManagement.API.MapperFunc;
using TaskManagement.API.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// Configuration
var config = builder.Configuration;
var jwtSecret = config["JwtSettings:Secret"];
var connectionString = config.GetConnectionString("DefaultConnection");

// -------------------------------
// Services
builder.WebHost.ConfigureKestrel(
    options =>
    {
        options.ListenLocalhost(7120, listenOptions =>
        {
            listenOptions.UseHttps(); // Enable HTTPS
        });
    });

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 1000,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});
builder.Services.AddSignalR();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddAutoMapper(typeof(TaskMappingProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// -------------------------------
// Authentication - JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JwtSettings:Issuer"],
            ValidAudience = config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!))
        };
    });
// -------------------------------
// CUSTOM EXCEPTION
builder.Services.AddScoped<CustomExceptionFilter>();
// -------------------------------
// SERILOG
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(); 

// -------------------------------
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowHtmlTestClient", builder =>
    {
        builder.WithOrigins("http://localhost:64855") // updated port
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



// -------------------------------
// Swagger
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Management API", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });

    opt.OperationFilter<FileUploadOperationFilter>();
});

builder.Services.AddControllers();


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new
        {
            success = false,
            message = "Validation failed",
            data = (object?)null,
            errors
        };

        return new BadRequestObjectResult(response);
    };
});

// -------------------------------
// Build the App
var app = builder.Build();

// -------------------------------
// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("AllowHtmlTestClient");

app.UseAuthorization();
app.MapHub<TaskHub>("/hubs/tasks");
app.UseRateLimiter();
app.MapControllers();
app.Run();

// for to run front end serve -l 64855