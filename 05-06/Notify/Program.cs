using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Notify.Contexts;
using Notify.Interfaces;
using Notify.Misc;
using Notify.Models;
using Notify.Repositories;
using Notify.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Notify App", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please Enter Token ",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddControllers(options =>
                    {
                        options.Filters.Add<CustomExceptionFilter>();
                    })
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                        options.JsonSerializerOptions.WriteIndented = true;
                    });

builder.Services.AddDbContext<NotifyContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});


#region Repositories
builder.Services.AddTransient<IRepo<int, User>, UserRepo>();
builder.Services.AddTransient<IRepo<string, Document>, DocumentRepo>();
#endregion

#region Services
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<DocumentService>();
builder.Services.AddTransient<HashingService>();
builder.Services.AddTransient<TokenService>();
builder.Services.AddTransient<AuthenticationService>();
#endregion

#region Misc
builder.Services.AddScoped<ExceptionFilterAttribute, CustomExceptionFilter>();

// builder.Services.AddSingleton<NotificationHub>();
#endregion

#region Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]!))
                        };
                    });
builder.Services.AddAuthorization();
#endregion 

builder.Services.AddSignalR();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notification");
app.MapControllers();

app.Run();
