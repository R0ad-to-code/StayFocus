using FocusGuardApi.DTOs;
using FocusGuardApi.Models;

namespace FocusGuardApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, AuthResponseDto Response)> RegisterAsync(RegisterUserDto registerDto);
        Task<(bool Success, string Message, AuthResponseDto Response)> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
}
