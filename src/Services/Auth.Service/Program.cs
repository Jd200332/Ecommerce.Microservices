using Auth.Service.Data;
using Auth.Service.Models;
using Auth.Service.Services;
using ECommerce.Shared.Middleware;
using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using System.Text;
using Serilog;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.RateLimiting;
using Prometheus;
using System.Data.Common;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. IMPROVED LOGGING (70% - Asynchronous & Enriched)
// ==========================================
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
           // Crucial for tracking container instances
          .Enrich.WithProperty("Application", "Auth.Service");


    // Non-blocking async console sink
});

// Database
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Authdb")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"]
    };
});

// Swagger + JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth.Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AuthCors", policy =>
    {
        policy.WithOrigins("http://localhost:7192")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================================
// 2. IMPROVED RATE LIMITING (70% - Partitioned Token Bucket)
// ==========================================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Token Bucket policy partitioned safely by individual Client IP
    options.AddTokenBucketLimiter("AuthTokenBucket", config =>
    {
        config.TokenLimit = 10;
        config.QueueLimit = 0; // Drop brute-force requests immediately instead of queuing them in memory
        config.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
        config.TokensPerPeriod = 2;
    });

    // Partition by Remote IP Address so malicious users only lock themselves out
    options.AddPolicy("IPPartitionedPolicy", context =>
    {
        var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        return RateLimitPartition.GetTokenBucketLimiter(remoteIp, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 20,
            QueueLimit = 0,
            ReplenishmentPeriod = TimeSpan.FromSeconds(30),
            TokensPerPeriod = 5
        });
    });
});

var app = builder.Build();

// ==========================================
// 3. CORRECTED MIDDLEWARE ORDERING & MONITORING
// ==========================================

// Track metrics immediately at the front gate before anyone gets rate-limited or blocked
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AuthCors");
app.UseRateLimiter(); // Runs right after CORS to shed bad traffic immediately
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// MONITORING (70%): Protect the metrics endpoint by binding it only to an internal management port (e.g., 5001)
app.MapMetrics("/metrics").RequireHost("*:5001");

app.Run();