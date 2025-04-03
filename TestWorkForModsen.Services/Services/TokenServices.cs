using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Options;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Data.Repository.BasicInterfaces;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAccountRepository<Account> _accountRepository;

        public TokenService(
            IOptions<JwtSettings> jwtSettings,
            IRefreshTokenRepository refreshTokenRepository,
            IAccountRepository<Account> accountRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _accountRepository = accountRepository;
        }

        public string GenerateJwtToken(Account account)
        {
            try
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
            catch (Exception ex)
            {
                throw new CustomTokenGenerationException("Ошибка генерации JWT токена", ex);
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomBytes = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
            catch (Exception ex)
            {
                throw new CustomTokenGenerationException("Ошибка генерации refresh token", ex);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<string> GenerateAndSaveRefreshTokenAsync(int accountId)
        {
            try
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
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при создании refresh token", ex);
            }
        }

        public async Task<(Account account, string newRefreshToken)> RefreshTokenPairAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new CustomBadRequestException("Refresh token не может быть пустым");

            var tokenEntity = await _refreshTokenRepository.GetValidTokenAsync(refreshToken)
                ?? throw new CustomUnauthorizedException("Недействительный или просроченный refresh token");

            var account = await _accountRepository.GetByIdAsync(tokenEntity.AccountId)
                ?? throw new CustomNotFoundException("Аккаунт не найден");

            var newRefreshToken = await UpdateRefreshToken(tokenEntity);

            return (account, newRefreshToken);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (token == null) return false;

            await _refreshTokenRepository.DeleteAsync(token);
            return true;
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetValidTokenAsync(refreshToken);
            return token != null;
        }

        private async Task<string> UpdateRefreshToken(RefreshToken token)
        {
            try
            {
                var oldToken = token;
                token.Token = GenerateRefreshToken();
                token.ExpiryTime = DateTime.UtcNow.AddDays(7);
                await _refreshTokenRepository.UpdateAsync(token);

                await _refreshTokenRepository.DeleteAsync(oldToken);
                return token.Token;
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при обновлении refresh token", ex);
            }
        }
    }
}