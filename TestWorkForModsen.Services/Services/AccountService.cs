using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IRepository<Account> accountRepository, IMapper mapper)
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
            return account == null ? null : _mapper.Map<AccountResponseDto>(account);
        }

        public async Task<AccountResponseDto?> GetAccountByEmailAsync(string email)
        {
            var account = await _accountRepository.GetByEmailAsync(email);
            return account == null ? null : _mapper.Map<AccountResponseDto>(account);
        }

        public async Task<AccountResponseDto> CreateAccountAsync(AccountDto accountDto)
        {
            var account = _mapper.Map<Account>(accountDto);
            await _accountRepository.AddAsync(account);
            return _mapper.Map<AccountResponseDto>(account);
        }

        public async Task UpdateAccountAsync(int id, AccountDto accountDto)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found");
            }

            _mapper.Map(accountDto, account);
            await _accountRepository.UpdateAsync(account);
        }

        public async Task DeleteAccountAsync(int id)
        {
            await _accountRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AccountResponseDto>> GetPagedAccountsAsync(PaginationDto paginationDto)
        {
            var accounts = await _accountRepository.GetPagedAsync(paginationDto.PageNumber, paginationDto.PageSize);
            return _mapper.Map<IEnumerable<AccountResponseDto>>(accounts);
        }
    }
}
