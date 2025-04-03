using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Services.Validators;
using TestWorkForModsen.Services.Services;

namespace TestWorkForModsen.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository<User> _userRepository;
        private readonly IAccountRepository<Account> _accountRepository;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<RegisterRequestDto> _registerValidator;

        public AuthService(
            IUserRepository<User> userRepository,
            IAccountRepository<Account> accountRepository,
            IPasswordHasher<Account> passwordHasher,
            ITokenService tokenService,
            IMapper mapper,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<RegisterRequestDto> registerValidator)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _mapper = mapper;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        public async Task<TokenResponse> LoginAsync(LoginRequestDto request)
        {
            await _loginValidator.ValidateAndThrowAsync(request);

            var account = await _accountRepository.GetByEmailAsync(request.Email);
            if (account == null)
            {
                throw new CustomUnauthorizedException("Неверные учетные данные");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(account, account.Password, request.Password);
            if (verificationResult != PasswordVerificationResult.Success)
            {
                throw new CustomUnauthorizedException("Неверные учетные данные");
            }

            var accessToken = _tokenService.GenerateJwtToken(account);
            var refreshToken = await _tokenService.GenerateAndSaveRefreshTokenAsync(account.Id);

            return new TokenResponse(accessToken, refreshToken);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new CustomBadRequestException("Refresh token не может быть пустым");
            }

            var (account, newRefreshToken) = await _tokenService.RefreshTokenPairAsync(refreshToken);
            var newAccessToken = _tokenService.GenerateJwtToken(account);

            return new TokenResponse(newAccessToken, newRefreshToken);
        }

        public async Task<TokenResponse> RegisterAsync(RegisterRequestDto request)
        {
            await _registerValidator.ValidateAndThrowAsync(request);

            if (await _accountRepository.GetByEmailAsync(request.Email) != null)
            {
                throw new CustomConflictException("Пользователь с таким email уже существует");
            }

            try
            {
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

                var accessToken = _tokenService.GenerateJwtToken(account);
                var refreshToken = await _tokenService.GenerateAndSaveRefreshTokenAsync(account.Id);

                return new TokenResponse(accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при регистрации пользователя", ex);
            }
        }
    }
}