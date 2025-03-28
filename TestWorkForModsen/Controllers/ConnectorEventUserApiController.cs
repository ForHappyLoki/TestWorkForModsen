using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            return Ok(record);
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

        [Authorize(Policy = "AnyAuthenticated")]
        [HttpPost]
        public async Task<ActionResult<ConnectorEventUserResponseDto>> Create([FromBody] ConnectorEventUserCreateDto dto)
        {
            await _validator.ValidateAndThrowAsync(dto);
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetByCompositeKey),
                new { eventId = result.EventId, userId = result.UserId },
                result);
        }

        [Authorize(Policy = "AnyAuthenticated")]
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
            await _paginationValidator.ValidateAndThrowAsync(pagination);
            var records = await _service.GetPagedAsync(pagination);
            return Ok(records);
        }
    }
}