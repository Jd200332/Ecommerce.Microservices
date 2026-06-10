using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Product.Service.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ======================= DB =======================
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Productdb")));


// =================== Controllers =================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//ratelimiting 
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("FixedWindow", context =>
        RateLimitPartition.GetFixedWindowLimiter(partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown", factory: partition => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10, // Max 100 requests
            Window = TimeSpan.FromMilliseconds(20), // Per 1 minute
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0 // No queuing, reject immediately when limit is exceeded
        }));
});

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
