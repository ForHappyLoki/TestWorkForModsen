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
    public class EventApiController : ControllerBase
    {
        private readonly IEventService _service;
        private readonly IValidator<PaginationDto> _paginationValidator;

        public EventApiController(
            IEventService service,
           IValidator<PaginationDto> paginationValidator)
        {
            _service = service;
            _paginationValidator = paginationValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetAll()
        {
            var events = await _service.GetAllAsync();
            return Ok(events);
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<EventResponseDto>> GetById(int id)
        {
            var eventDto = await _service.GetByIdAsync(id);
            return eventDto == null ? NotFound() : Ok(eventDto);
        }

        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> Create([FromBody] EventCreateDto dto)
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
        public async Task<IActionResult> Update(int id, [FromBody] EventUpdateDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetPaged(
            [FromQuery] PaginationDto pagination)
        {
            var validationResult = await _paginationValidator.ValidateAsync(pagination);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var events = await _service.GetPagedAsync(pagination);
            return Ok(events);
        }
    }
}
