using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestWork_Events.Data;
using TestWork_Events.ModelView;
using TestWork_Events.Models;
using TestWork_Events.Options;
using TestWorkForModsen.Repository;

namespace TestWorkForModsen.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PasswordHasher<Account> _passwordHasher = new();

        public AuthService(IAuthRepository authRepository, IHttpClientFactory httpClientFactory)
        {
            _authRepository = authRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("AccountApi");
            var response = await httpClient.GetAsync($"email/{request.Email}");
            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("Пользователь не найден.");
            }

            var account = await response.Content.ReadFromJsonAsync<Account>();
            if (account == null)
            {
                throw new UnauthorizedAccessException("Пользователь не найден.");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(account, account.Password, request.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Неверный пароль.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, request.Email),
                new(ClaimTypes.Role, "User")
            };
            var accessToken = GenerateJwtToken(claims);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiryTime = DateTime.UtcNow.AddDays(7),
                AccountId = account.Id
            };
            await _authRepository.AddRefreshTokenAsync(refreshTokenEntity);

            return (accessToken, refreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string token)
        {
            var refreshTokenEntity = await _authRepository.GetRefreshTokenAsync(token);
            if (refreshTokenEntity == null || refreshTokenEntity.ExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Недействительный refresh token.");
            }

            var httpClient = _httpClientFactory.CreateClient("AccountApi");
            var accountResponse = await httpClient.GetAsync($"/api/AccountApi/{refreshTokenEntity.AccountId}");
            if (!accountResponse.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("Пользователь не найден.");
            }

            var account = await accountResponse.Content.ReadFromJsonAsync<Account>();
            if (account == null)
            {
                throw new UnauthorizedAccessException("Пользователь не найден.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, account.Email),
                new(ClaimTypes.Role, "User")
            };
            var accessToken = GenerateJwtToken(claims);

            var newRefreshToken = GenerateRefreshToken();
            refreshTokenEntity.Token = newRefreshToken;
            refreshTokenEntity.ExpiryTime = DateTime.UtcNow.AddDays(7);
            await _authRepository.UpdateRefreshTokenAsync(refreshTokenEntity);

            return (accessToken, newRefreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> RegisterAsync(RegisterRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("AccountApi");
            var checkResponse = await httpClient.GetAsync($"/api/AccountApi/email/{request.Email}");
            if (checkResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Пользователь с таким email уже существует.");
            }

            var hashedPassword = _passwordHasher.HashPassword(null, request.Password);

            var newUser = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Birthday = request.Birthday,
                Email = request.Email,
            };

            var createUserResponse = await httpClient.PostAsJsonAsync("/api/UserApi/", newUser);
            if (!createUserResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Ошибка при создании пользователя.");
            }

            var createdUser = await createUserResponse.Content.ReadFromJsonAsync<User>();
            if (createdUser == null)
            {
                throw new InvalidOperationException("Ошибка при создании пользователя.");
            }

            var newAccount = new Account
            {
                Email = request.Email,
                Password = hashedPassword,
                Role = "User",
                UserId = createdUser.Id,
                User = createdUser,
            };

            var createAccountResponse = await httpClient.PostAsJsonAsync("/api/AccountApi/", newAccount);
            if (!createAccountResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Ошибка при создании аккаунта.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, request.Email),
                new(ClaimTypes.Role, "User")
            };
            var accessToken = GenerateJwtToken(claims);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiryTime = DateTime.UtcNow.AddDays(7),
                AccountId = createdUser.Id
            };
            await _authRepository.AddRefreshTokenAsync(refreshTokenEntity);

            return (accessToken, refreshToken);
        }

        private static string GenerateJwtToken(List<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}