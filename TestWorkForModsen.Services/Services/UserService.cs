using AutoMapper;
using FluentValidation;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository<User> _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserCreateDto> _createValidator;
        private readonly IValidator<UserUpdateDto> _updateValidator;

        public UserService(
            IUserRepository<User> repository,
            IMapper mapper,
            IValidator<UserCreateDto> createValidator,
            IValidator<UserUpdateDto> updateValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Any()
                ? _mapper.Map<IEnumerable<UserResponseDto>>(users)
                : throw new CustomNotFoundException("Пользователи не найдены");
        }

        public async Task<UserResponseDto> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user != null
                ? _mapper.Map<UserResponseDto>(user)
                : throw new CustomNotFoundException($"Пользователь с ID {id} не найден");
        }

        public async Task<UserResponseDto> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new CustomBadRequestException("Email не может быть пустым");

            var user = await _repository.GetByEmailAsync(email);
            return user != null
                ? _mapper.Map<UserResponseDto>(user)
                : throw new CustomNotFoundException($"Пользователь с email {email} не найден");
        }

        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            if (await _repository.GetByEmailAsync(dto.Email) != null)
                throw new CustomConflictException($"Пользователь с email {dto.Email} уже существует");

            try
            {
                var user = _mapper.Map<User>(dto);
                await _repository.AddAsync(user);
                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при создании пользователя", ex);
            }
        }

        public async Task UpdateAsync(UserUpdateDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var existingUser = await _repository.GetByIdAsync(dto.Id)
                ?? throw new CustomNotFoundException($"Пользователь с ID {dto.Id} не найден");

            try
            {
                _mapper.Map(dto, existingUser);
                await _repository.UpdateAsync(existingUser);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при обновлении пользователя", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var userExists = await _repository.GetByIdAsync(id);
            if (userExists == null)
                throw new CustomNotFoundException($"Пользователь с ID {id} не найден");

            try
            {
                await _repository.DeleteAsync(userExists);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при удалении пользователя", ex);
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            if (pagination.PageNumber < 1 || pagination.PageSize < 1)
                throw new CustomBadRequestException("Некорректные параметры пагинации");

            var users = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return users.Any()
                ? _mapper.Map<IEnumerable<UserResponseDto>>(users)
                : throw new CustomNotFoundException("Пользователи не найдены для указанной страницы");
        }
    }
}