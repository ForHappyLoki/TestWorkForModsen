using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Models.Validators;
using TestWorkForModsen.Services.Services;

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
            return user == null ? NotFound() : Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserResponseDto>> GetByEmail(string email)
        {
            var user = await _service.GetByEmailAsync(email);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserCreateDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (ValidationException ex)
            {
                throw new ValidationException(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetPaged(
            [FromQuery] PaginationDto pagination)
        {
            var validationResult = await _paginationValidator.ValidateAsync(pagination);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var users = await _service.GetPagedAsync(pagination);
            return Ok(users);
        }
    }
}
