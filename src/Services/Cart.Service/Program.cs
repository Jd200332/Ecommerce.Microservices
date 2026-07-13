using Cart.Service.Data;
using Cart.Service.Services;
using ECommerce.MessageBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
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

// Register HttpClient for ProductCatalogClient
builder.Services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>(client =>
{
    var baseUrl = builder.Configuration["ProductService:BaseUrl"]
        ?? throw new InvalidOperationException("ProductService:BaseUrl is missing");
    client.BaseAddress = new Uri(baseUrl);
});

// Register your services


builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAdminAccess, CartService>();

// IF CartService implements both ICartService and IAdminAccess, this is fine.
// Ensure IProductCatalogClient is registered (if used in CartService constructor).
// If CartService depends on IProductCatalogClient, you need to register it.
// Example:
// builder.Services.AddScoped<IProductCatalogClient, ProductCatalogClient>();


// RabbitMQ
var rabbitMqHost = builder.Configuration["RabbitMQ:Host"];
if (string.IsNullOrEmpty(rabbitMqHost))
{
    throw new Exception("RabbitMQ:Host is missing in appsettings.json");
}
builder.Services.AddSingleton<IMessageBus>(sp =>
    new RabbitMQMessageBus(rabbitMqHost));

// Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("read", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(1);
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

// Middleware
app.UseRouting();
app.UseRateLimiter();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.Run();