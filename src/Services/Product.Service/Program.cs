using Microsoft.EntityFrameworkCore;
using Product.Service.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDbContext<ProductDbContext>
 (options => options.UseSqlServer(builder.Configuration.GetConnectionString("Productdb")));

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




