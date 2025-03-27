using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<UserResponseDto?> GetByEmailAsync(string email);
        Task<UserResponseDto> CreateAsync(UserCreateDto dto);
        Task UpdateAsync(int id, UserUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<UserResponseDto>> GetPagedAsync(PaginationDto pagination);
    }
}
