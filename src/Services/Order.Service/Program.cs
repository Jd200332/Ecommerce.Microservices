using Microsoft.EntityFrameworkCore;
using Order.Service.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>
    (options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("Orderdb")));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware (only in development)
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
