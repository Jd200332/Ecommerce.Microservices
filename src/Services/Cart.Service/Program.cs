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
        builder.Configuration.GetConnectionString("Cartdb")));

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

// RabbitMQ
var rabbitMqHost = builder.Configuration["RabbitMQ:Host"];
if (string.IsNullOrEmpty(rabbitMqHost))
{
    //throw new Exception("RabbitMQ:Host is missing in appsettings.json");
    rabbitMqHost = "localhost";
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

// =================== MIDDLEWARE ==================
// ✅ SWAGGER – ONLY IN DEVELOPMENT
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.MapControllers();


app.Run();