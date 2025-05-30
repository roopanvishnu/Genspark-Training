using Bank.Contexts;
using Bank.Interfaces;
using Bank.Repositories;
using Bank.Services;
using BankChatApplication;
using BankChatApplication.Interfaces;
using BankChatApplication.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Chat App Config =====
builder.Services.Configure<ApiContext>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatService, ChatService>();

// ===== Banking App Config =====
builder.Services.AddDbContext<BankContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBankRepository, BankRepositories>();
builder.Services.AddScoped<IBankService, BankService>();

// ===== Common Setup =====
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Middleware =====
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();