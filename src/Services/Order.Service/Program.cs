using ECommerce.MessageBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Order.Service.Data;
using Order.Service.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================= DB =======================
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Orderdb")));

// =================== Controllers =================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ==================== Swagger + JWT ====================
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ======================= DI =======================
builder.Services.AddScoped<IOrderService, OrderService>();

// ==================== HttpClient =================
builder.Services.AddHttpClient("ProductService", c =>
{
    c.BaseAddress = new Uri("https://localhost:7397"); // ProductService
});

// ===================== RabbitMQ ==================
builder.Services.AddSingleton<IMessageBus>(
    _ => new RabbitMQMessageBus("localhost"));

// ===================== JWT =======================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = true,
        ValidateAudience = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"]
    };
});

builder.Services.AddAuthorization();

// ====================== App ======================
var app = builder.Build();

// =================== Middleware ==================
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();   // MUST be before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
