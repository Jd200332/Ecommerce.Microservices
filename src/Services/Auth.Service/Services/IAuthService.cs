using Auth.Service.Dtos;

namespace Auth.Service.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);    
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<object> GetProfileAsync(string userId);
    }
}