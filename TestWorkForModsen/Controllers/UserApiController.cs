using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Services.Services;
using Microsoft.AspNetCore.Authorization;

namespace TestWorkForModsen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IValidator<PaginationDto> _paginationValidator;

        public UserApiController(
            IUserService service,
            IValidator<PaginationDto> paginationValidator)
        {
            _service = service;
            _paginationValidator = paginationValidator;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserResponseDto>> GetByEmail(string email)
        {
            var user = await _service.GetByEmailAsync(email);
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Policy = "AnyAuthenticated")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [Authorize(Policy = "AnyAuthenticated")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        [Authorize(Policy = "AnyAuthenticated")]
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetPaged(
            [FromQuery] PaginationDto pagination)
        {
            await _paginationValidator.ValidateAndThrowAsync(pagination);
            var users = await _service.GetPagedAsync(pagination);
            return Ok(users);
        }
    }
}
