using System.Text;
using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop API", Version = "v1" });
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

builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    }
);

builder.Services.AddDbContext<ChienVHShopDBEntities>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
    }
);

builder.Services.AddTransient<CategoryRepo>();
builder.Services.AddTransient<ColorRepo>();
builder.Services.AddTransient<ContactURepo>();
builder.Services.AddTransient<ModelRepo>();
builder.Services.AddTransient<NewsRepo>();
builder.Services.AddTransient<OrderDetailRepo>();
builder.Services.AddTransient<OrderRepo>();
builder.Services.AddTransient<ProductRepo>();
builder.Services.AddTransient<UserRepo>();

builder.Services.AddTransient<CategoryService>();
builder.Services.AddTransient<ColorService>();
builder.Services.AddTransient<ContactUsService>();
builder.Services.AddTransient<NewsService>();
builder.Services.AddTransient<OrderService>();
builder.Services.AddTransient<ProductService>();
builder.Services.AddTransient<ShoppingCartService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<AuthService>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSecret"]!))
                        };
                    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
