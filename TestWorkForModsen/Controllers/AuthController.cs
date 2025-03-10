using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestWork_Events.Data;
using TestWork_Events.Models;
using TestWork_Events.Options;

namespace TestWork_Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(DatabaseContext databaseContext, IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private readonly DatabaseContext _databaseContext = databaseContext;
        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>(); 
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AccountApi");

        // Метод, выдающий ошибку. Создан для проверки работоспособности мидлваре, отрабатывающий эту ситуацию
        [HttpGet("throw")]
        public IActionResult GetName()
        {
            throw new Exception("Тестовое исключение для Middleware.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TestWork_Events.ModelView.LoginRequest request)
        {
            // Запрос к AccountApi для получения аккаунта по email
            var response = await _httpClient.GetAsync($"email/{request.Email}");
            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(); // Возвращаем 401, если пользователь не найден
            }

            var account = await response.Content.ReadFromJsonAsync<Account>();
            if (account == null)
            {
                return Unauthorized(); // Возвращаем 401, если пользователь не найден
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(account, account.Password, request.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return Unauthorized(); // Возвращаем 401, если пароль не верный
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Role, "User")
            };
            var accessToken = GenerateJwtToken(claims);

            // Генерация refresh token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiryTime = DateTime.UtcNow.AddDays(7), // Срок жизни 7 дней
                AccountId = account.Id
            };
            _databaseContext.RefreshTokens.Add(refreshTokenEntity);
            await _databaseContext.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        private string GenerateJwtToken(List<Claim> claims)
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

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            var refreshTokenEntity = _databaseContext.RefreshTokens
                .FirstOrDefault(rt => rt.Token == token);

            if (refreshTokenEntity == null || refreshTokenEntity.ExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(); // если рефреш токен не найден или истёк
            }

            // Запрос к AccountApi для получения аккаунта по ID
            var accountResponse = await _httpClient.GetAsync($"/api/AccountApi/{refreshTokenEntity.AccountId}");
            if (!accountResponse.IsSuccessStatusCode)
            {
                return Unauthorized(); // если аккаунт не найден
            }

            var account = await accountResponse.Content.ReadFromJsonAsync<Account>();
            if (account == null)
            {
                return Unauthorized(); 
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, "User")
            };
            var accessToken = GenerateJwtToken(claims);

            var newRefreshToken = GenerateRefreshToken();

            refreshTokenEntity.Token = newRefreshToken;
            refreshTokenEntity.ExpiryTime = DateTime.UtcNow.AddDays(7);
            await _databaseContext.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] TestWork_Events.ModelView.RegisterRequest request)
        {
            var checkResponse = await _httpClient.GetAsync($"/api/AccountApi/email/{request.Email}");
            if (checkResponse.IsSuccessStatusCode)
            {
                return Conflict("Пользователь с таким email уже существует.");
            }

            var hashedPassword = _passwordHasher.HashPassword(null, request.Password);

            var newUser = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Birthday = request.Birthday,
                Email = request.Email,
            };

            // Создание нового юзера через API
            var createUser = await _httpClient.PostAsJsonAsync("/api/UserApi/", newUser);
            int userId;
            if (createUser.IsSuccessStatusCode)
            {
                var createdUser = await createUser.Content.ReadFromJsonAsync<User>();
                userId = createdUser.Id;
            }
            else
            {
                return StatusCode((int)createUser.StatusCode, $"Ошибка при создании пользователя: {createUser.StatusCode}");
            }

            var newAccount = new Account
            {
                Email = request.Email,
                Password = request.Password,
                Role = "User",
                UserId = userId,
                User = newUser,
            };

            // Создание нового аккаунта через API
            var createResponse = await _httpClient.PostAsJsonAsync("/api/AccountApi/", newAccount);
            if (!createResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)createResponse.StatusCode, "Ошибка при создании аккаунта.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Role, "User")
            };
            var accessToken = GenerateJwtToken(claims);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiryTime = DateTime.UtcNow.AddDays(7), 
                AccountId = userId 
            };

            _databaseContext.RefreshTokens.Add(refreshTokenEntity);
            await _databaseContext.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}