using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository.BasicInterfaces;
using TestWorkForModsen.Data.Repository;

namespace TestWorkForModsen.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository<Account> _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository<Account> accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AccountResponseDto>>(accounts);
        }

        public async Task<AccountResponseDto?> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new Exception($"Аккаунт с ID {id} не найден");
            }
            return _mapper.Map<AccountResponseDto>(account);
        }

        public async Task<AccountResponseDto?> GetAccountByEmailAsync(string email)
        {
            var account = await _accountRepository.GetByEmailAsync(email);
            if (account == null)
            {
                throw new Exception($"Аккаунт с email {email} не найден");
            }
            return _mapper.Map<AccountResponseDto>(account);
        }

        public async Task<AccountResponseDto> CreateAccountAsync(AccountDto accountDto)
        {
            var account = _mapper.Map<Account>(accountDto);
            await _accountRepository.AddAsync(account);
            return _mapper.Map<AccountResponseDto>(account);
        }

        public async Task UpdateAccountAsync(AccountDto accountDto)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(accountDto.UserId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found");
                }

                _mapper.Map(accountDto, account);
                await _accountRepository.UpdateAsync(account);
            }
            catch
            {
                throw new Exception("Не удалось обновить запись");
            }
        }

        public async Task DeleteAccountAsync(int id)
        {
            try
            {
                await _accountRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Не удалось удалить запись, {ex.Message}");
            }
        }

        public async Task<IEnumerable<AccountResponseDto>> GetPagedAccountsAsync(PaginationDto paginationDto)
        {
            var accounts = await _accountRepository.GetPagedAsync(paginationDto.PageNumber, paginationDto.PageSize);
            return _mapper.Map<IEnumerable<AccountResponseDto>>(accounts);
        }
    }
}
