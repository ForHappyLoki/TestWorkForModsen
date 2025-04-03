using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;
using System.Threading;

namespace TestWorkForModsen.Services.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync();
        Task<AccountResponseDto?> GetAccountByIdAsync(int id);
        Task<AccountResponseDto?> GetAccountByEmailAsync(string email);
        Task<AccountResponseDto> CreateAccountAsync(AccountDto accountDto);
        Task UpdateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken);
        Task DeleteAccountAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<AccountResponseDto>> GetPagedAccountsAsync(PaginationDto paginationDto);
    }
}
