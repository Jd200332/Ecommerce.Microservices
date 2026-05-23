using Auth.Service.Data;
using Auth.Service.Dtos;
using Auth.Service.Models;
using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly JwtSettings jwtSettings;
        private readonly ILogger<AuthService> logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtSettings = jwtSettings.Value;
            this.logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User already exists");

            if(request.PhoneNumber == null || request.PhoneNumber.Length > 10)
                throw new InvalidOperationException("Phone number cannot exceed 10 digits");


            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = "N/A",
                City = "N/A",
                State = "N/A",
                Country = "N/A",
                ZipCode = "0000000",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            var token = await GenerateJwtToken(user);
            logger.LogInformation("User registered: {Email}", user.Email);

            return new AuthResponse
            {
                Token = token,
                Userid = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.DurationInMinutes)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request) 
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials");

            if(user.LockoutEnabled && user.LockoutEnd > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Account is locked. Please try again later.");

            if(user.UserName == null)
            {
                logger.LogWarning("User {Email} has no username set", user.Email);
                throw new InvalidOperationException("User account is not properly configured");
            }

            var token = await GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Userid = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.DurationInMinutes)
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (user.FirstName == null && user.LastName == null)
                throw new InvalidOperationException
                    ("First and last name cannot be null");

            if(user.PhoneNumber.Length > 10)
                throw new InvalidOperationException
                    ("Phone number cannot exceed 10 digits");

            if(user.PhoneNumber == null)
                logger.LogWarning
                    ("User {UserId} has no phone number set", user.Id);


            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.Address = request.Address ?? user.Address;
            user.City = request.City ?? user.City;
            user.State = request.State ?? user.State;
            user.Country = request.Country ?? user.Country;
            user.ZipCode = request.ZipCode ?? user.ZipCode;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<object> GetProfileAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.Address,
                user.City,
                user.State,
                user.Country,
                user.ZipCode
            };


        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("userId", user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}