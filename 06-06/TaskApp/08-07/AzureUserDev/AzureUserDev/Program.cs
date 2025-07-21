using Microsoft.EntityFrameworkCore;
using AzureUserDev.data;
using AzureUserDev.model;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=20.102.88.64;Port=5432;Database=azuredb;Username=postgres;Password=1234";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


app.MapGet("/", () => "API is running!");

app.MapGet("/persons", async (AppDbContext db) =>
    await db.Persons.ToListAsync());

app.MapPost("/persons", async (Person person, AppDbContext db) =>
{
    db.Persons.Add(person);
    await db.SaveChangesAsync();
    return Results.Created($"/persons/{person.Id}", person);
});

app.Run();