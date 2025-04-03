using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Services.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(Account account);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        Task<string> GenerateAndSaveRefreshTokenAsync(int accountId);
        Task<(Account account, string newRefreshToken)> RefreshTokenPairAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);
    }
}
