using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequestDto request);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        Task<TokenResponse> RegisterAsync(RegisterRequestDto request);
    }

    public record TokenResponse(string AccessToken, string RefreshToken);
}