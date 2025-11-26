using Auth.Service.Dtos;
using Auth.Service.Services;
using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Service.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authservice;

        public AuthController(IAuthService authservice)
        {
            this.authservice = authservice;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
        {
            var result = await authservice.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResult
                (result , "Registration successful"));

            
        }

        [HttpPost("login")]
        
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
        {
            var result = await authservice.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResult(result,
                "Login successful"));  
        }


        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetProfile()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await authservice.GetProfileAsync(userid);
            return Ok(ApiResponse<object>.SuccessResult(
                profile));
        }


        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateProfile(UpdateProfileRequest request)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await authservice.UpdateProfileAsync(userid, request);
            return Ok(ApiResponse<bool>.SuccessResult(
                result, "Profile updated successfully"));
        }
    }
}