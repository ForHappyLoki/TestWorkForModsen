using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync();
        Task<AccountResponseDto?> GetAccountByIdAsync(int id);
        Task<AccountResponseDto?> GetAccountByEmailAsync(string email);
        Task<AccountResponseDto> CreateAccountAsync(AccountDto accountDto);
        Task UpdateAccountAsync(int id, AccountDto accountDto);
        Task DeleteAccountAsync(int id);
        Task<IEnumerable<AccountResponseDto>> GetPagedAccountsAsync(PaginationDto paginationDto);
    }
}
