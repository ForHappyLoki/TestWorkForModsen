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
            return Ok(eventDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> Create([FromBody] EventCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] EventUpdateDto dto, CancellationToken cancellationToken = default)
        {
            await _service.UpdateAsync(dto, cancellationToken);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetPaged(
            [FromQuery] PaginationDto pagination)
        {
            await _paginationValidator.ValidateAndThrowAsync(pagination);
            var events = await _service.GetPagedAsync(pagination);
            return Ok(events);
        }
    }
}
