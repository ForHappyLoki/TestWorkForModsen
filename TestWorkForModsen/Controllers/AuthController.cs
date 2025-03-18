using Microsoft.AspNetCore.Mvc;
using TestWork_Events.ModelView;
using TestWorkForModsen.Services;

namespace TestWork_Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (accessToken, refreshToken) = await _authService.LoginAsync(request);
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            var (accessToken, refreshToken) = await _authService.RefreshTokenAsync(token);
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (accessToken, refreshToken) = await _authService.RegisterAsync(request);
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }
    }
}