using FirstAPI.Repository;
using FirstAPI.Repository.Interface;
using FirstAPI.Services;
using FirstAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register dependencies
builder.Services.AddSingleton<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddSingleton<IAppointmentService, AppointmentService>();

var app = builder.Build();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();