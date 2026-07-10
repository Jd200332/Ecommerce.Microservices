using Cart.Service.Data;
using Cart.Service.Services;
using ECommerce.MessageBus;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CartDb")));

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAdminAccess, CartService>();
builder.Services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>(client =>
{
    var productServiceUrl = builder.Configuration["ProductService:BaseUrl"]
        ?? throw new InvalidOperationException("ProductService:BaseUrl is not configured");

    client.BaseAddress = new Uri(productServiceUrl);
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("read", opt =>
    {
        opt.PermitLimit = 5;  // SET TO 5 FOR TESTING - YOU'LL SEE 429 IMMEDIATELY
        opt.Window = TimeSpan.FromSeconds(1);
        opt.QueueLimit = 0;
    });
});


var app = builder.Build();

app.UseRateLimiter();
app.MapControllers();   


// Middleware
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();




app.Run();
