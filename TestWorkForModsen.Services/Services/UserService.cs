using AutoMapper;
using FluentValidation;
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
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception($"Пользователь с ID {id} не найден");
            }
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto?> GetByEmailAsync(string email)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception($"Пользователь с email {email} не найден");
            }
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            try
            {
                await _createValidator.ValidateAndThrowAsync(dto);

                var user = _mapper.Map<User>(dto);
                await _repository.AddAsync(user);

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания юзера, {ex.Message}");
            }
        }

        public async Task UpdateAsync(UserUpdateDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);
            try
            {
                var user = await _repository.GetByIdAsync(dto.Id);
                if (user == null) throw new KeyNotFoundException("Пользователь не найден");

                _mapper.Map(dto, user);
                await _repository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания пользователя, {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления пользователя, {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            var users = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
    }
}
