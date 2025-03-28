using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using TestWorkForModsen.Options;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Data.Repository.BasicInterfaces;

namespace TestWorkForModsen.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository<User> _userRepository;
        private readonly IAccountRepository<Account> _accountRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<RegisterRequestDto> _registerValidator;

        public AuthService(
            IUserRepository<User> userRepository,
            IAccountRepository<Account> accountRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IOptions<JwtSettings> jwtSettings,
            IMapper mapper,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<RegisterRequestDto> registerValidator,
            IPasswordHasher<Account> passwordHasher)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtSettings = jwtSettings.Value;
            _mapper = mapper;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _passwordHasher = passwordHasher;
        }

        public async Task<TokenResponse> LoginAsync(LoginRequestDto request)
        {
            await _loginValidator.ValidateAndThrowAsync(request);

            var account = await _accountRepository.GetByEmailAsync(request.Email)
                ?? throw new UnauthorizedAccessException("Пользователь не найден");

            if (account.Password != request.Password)
            {
                throw new UnauthorizedAccessException("Неверный пароль");
            }

            var accessToken = GenerateJwtToken(account);
            var refreshToken = await GenerateAndSaveRefreshToken(account.Id);

            return new TokenResponse(accessToken, refreshToken);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _refreshTokenRepository.GetValidTokenAsync(refreshToken)
                ?? throw new UnauthorizedAccessException("Недействительный или просроченный refresh token");

            var account = await _accountRepository.GetByIdAsync(tokenEntity.AccountId)
                ?? throw new UnauthorizedAccessException("Аккаунт не найден");

            var newAccessToken = GenerateJwtToken(account);
            var newRefreshToken = await UpdateRefreshToken(tokenEntity);

            return new TokenResponse(newAccessToken, newRefreshToken);
        }

        public async Task<TokenResponse> RegisterAsync(RegisterRequestDto request)
        {
            await _registerValidator.ValidateAndThrowAsync(request);

            if (await _accountRepository.GetByEmailAsync(request.Email) == null)
            {
                throw new InvalidOperationException("Пользователь с таким email уже существует");
            }

            var user = _mapper.Map<User>(request);
            await _userRepository.AddAsync(user);

            var account = new Account
            {
                Email = request.Email,
                Password = _passwordHasher.HashPassword(null, request.Password),
                Role = "User",
                UserId = user.Id
            };
            await _accountRepository.AddAsync(account);

            var accessToken = GenerateJwtToken(account);
            var refreshToken = await GenerateAndSaveRefreshToken(account.Id);

            return new TokenResponse(accessToken, refreshToken);
        }

        private string GenerateJwtToken(Account account)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Email, account.Email),
            new Claim(ClaimTypes.Role, account.Role),
            new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateAndSaveRefreshToken(int accountId)
        {
            var refreshToken = GenerateRefreshToken();
            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                AccountId = accountId,
                ExpiryTime = DateTime.UtcNow.AddDays(7)
            });
            return refreshToken;
        }

        private async Task<string> UpdateRefreshToken(RefreshToken token)
        {
            token.Token = GenerateRefreshToken();
            token.ExpiryTime = DateTime.UtcNow.AddDays(7);
            await _refreshTokenRepository.UpdateAsync(token);
            return token.Token;
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}