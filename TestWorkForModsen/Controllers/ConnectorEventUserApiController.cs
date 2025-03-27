using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Models.Validators;
using TestWorkForModsen.Services.Services;

namespace TestWorkForModsen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectorEventUserController : ControllerBase
    {
        private readonly IConnectorEventUserService _service;
        private readonly IValidator<ConnectorEventUserCreateDto> _validator;
        private readonly IValidator<PaginationDto> _paginationValidator;

        public ConnectorEventUserController(
            IConnectorEventUserService service,
            IValidator<ConnectorEventUserCreateDto> validator,
            IValidator<PaginationDto> paginationValidator)
        {
            _service = service;
            _validator = validator;
            _paginationValidator = paginationValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConnectorEventUserResponseDto>>> GetAll()
        {
            var records = await _service.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("{eventId}/{userId}")]
        public async Task<ActionResult<ConnectorEventUserResponseDto>> GetByCompositeKey(int eventId, int userId)
        {
            var record = await _service.GetByCompositeKeyAsync(eventId, userId);
            return record == null ? NotFound() : Ok(record);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ConnectorEventUserResponseDto>>> GetAllByUserId(int userId)
        {
            var records = await _service.GetAllByUserIdAsync(userId);
            return Ok(records);
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<ConnectorEventUserResponseDto>>> GetAllByEventId(int eventId)
        {
            var records = await _service.GetAllByEventIdAsync(eventId);
            return Ok(records);
        }

        [HttpPost]
        public async Task<ActionResult<ConnectorEventUserResponseDto>> Create([FromBody] ConnectorEventUserCreateDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetByCompositeKey),
                    new { eventId = result.EventId, userId = result.UserId },
                    result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{eventId}/{userId}")]
        public async Task<IActionResult> Delete(int eventId, int userId)
        {
            await _service.DeleteByCompositeKeyAsync(eventId, userId);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<ConnectorEventUserResponseDto>>> GetPaged(
            [FromQuery] PaginationDto pagination)
        {
            var validationResult = await _paginationValidator.ValidateAsync(pagination);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var records = await _service.GetPagedAsync(pagination);
            return Ok(records);
        }
    }
}