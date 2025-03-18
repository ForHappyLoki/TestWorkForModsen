using TestWork_Events.ModelView;
using TestWork_Events.Models;

namespace TestWorkForModsen.Services
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginRequest request);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string token);
        Task<(string AccessToken, string RefreshToken)> RegisterAsync(RegisterRequest request);
    }
}