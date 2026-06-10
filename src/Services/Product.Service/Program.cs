using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Product.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// ======================= DB =======================
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Productdb")));

// =================== Controllers =================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================= DI =======================
builder.Services.AddScoped<IProductService, ProductService>();



var app = builder.Build();

// =================== Middleware ==================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
