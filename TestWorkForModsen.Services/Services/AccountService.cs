using AutoMapper;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Core.Exceptions;
using System.Net;

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
            return accounts.Any()
                ? _mapper.Map<IEnumerable<AccountResponseDto>>(accounts)
                : throw new CustomNotFoundException("Аккаунты не найдены");
        }

        public async Task<AccountResponseDto> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            return account != null
                ? _mapper.Map<AccountResponseDto>(account)
                : throw new CustomNotFoundException($"Аккаунт с ID {id} не найден");
        }

        public async Task<AccountResponseDto> GetAccountByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new CustomBadRequestException("Email не может быть пустым");

            var account = await _accountRepository.GetByEmailAsync(email);
            return account != null
                ? _mapper.Map<AccountResponseDto>(account)
                : throw new CustomNotFoundException($"Аккаунт с email {email} не найден");
        }

        public async Task<AccountResponseDto> CreateAccountAsync(AccountDto accountDto)
        {
            if (accountDto == null)
                throw new CustomBadRequestException("Данные аккаунта не могут быть null");

            var existingAccount = await _accountRepository.GetByEmailAsync(accountDto.Email);
            if (existingAccount != null)
                throw new CustomConflictException($"Аккаунт с email {accountDto.Email} уже существует");

            try
            {
                var account = _mapper.Map<Account>(accountDto);
                await _accountRepository.AddAsync(account);
                return _mapper.Map<AccountResponseDto>(account);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при создании аккаунта", ex);
            }
        }

        public async Task UpdateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
        {
            if (accountDto == null)
                throw new CustomBadRequestException("Данные аккаунта не могут быть null");

            var existingAccount = await _accountRepository.GetByIdAsync(accountDto.UserId)
                ?? throw new CustomNotFoundException($"Аккаунт с ID {accountDto.UserId} не найден");

            try
            {
                _mapper.Map(accountDto, existingAccount);
                await _accountRepository.UpdateAsync(existingAccount, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при обновлении аккаунта", ex);
            }
        }

        public async Task DeleteAccountAsync(int id, CancellationToken cancellationToken = default)
        {
            var accountExists = await _accountRepository.GetByIdAsync(id);
            if (accountExists == null)
            {
                throw new CustomNotFoundException($"Аккаунт с ID {id} не найден");
            }

            try
            {
                await _accountRepository.DeleteAsync(accountExists, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при удалении аккаунта", ex);
            }
        }

        public async Task<IEnumerable<AccountResponseDto>> GetPagedAccountsAsync(PaginationDto paginationDto)
        {
            if (paginationDto == null || paginationDto.PageNumber < 1 || paginationDto.PageSize < 1)
                throw new CustomBadRequestException("Некорректные параметры пагинации");

            var accounts = await _accountRepository.GetPagedAsync(paginationDto.PageNumber, paginationDto.PageSize);
            return accounts.Any()
                ? _mapper.Map<IEnumerable<AccountResponseDto>>(accounts)
                : throw new CustomNotFoundException("Аккаунты не найдены для указанной страницы");
        }
    }
}