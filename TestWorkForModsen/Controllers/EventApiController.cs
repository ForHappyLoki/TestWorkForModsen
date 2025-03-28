using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Models.Validators;
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

        [Authorize(Policy = "AnyAuthenticated")]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> Create([FromBody] EventCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Policy = "AnyAuthenticated")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] EventUpdateDto dto)
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
