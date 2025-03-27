using AutoMapper;
using FluentValidation;
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
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserCreateDto> _createValidator;
        private readonly IValidator<UserUpdateDto> _updateValidator;

        public UserService(
            IRepository<User> repository,
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
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto?> GetByEmailAsync(string email)
        {
            var user = await _repository.GetByEmailAsync(email);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var user = _mapper.Map<User>(dto);
            await _repository.AddAsync(user);

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task UpdateAsync(int id, UserUpdateDto dto)
        {
            if (id != dto.Id) throw new ArgumentException("ID в пути и теле запроса не совпадают");

            await _updateValidator.ValidateAndThrowAsync(dto);

            var user = await _repository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("Пользователь не найден");

            _mapper.Map(dto, user);
            await _repository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            var users = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
    }
}
