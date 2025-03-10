using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace TestWork_Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectorEventUserController(IConnectorEventUserRepository<ConnectorEventUser> repository) : ControllerBase
    {
        private readonly IConnectorEventUserRepository<ConnectorEventUser> _repository = repository;

        // Получить все записи
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConnectorEventUser>>> GetAll()
        {
            var records = await _repository.GetAllAsync();
            return Ok(records);
        }

        // Получить запись по составному ключу (EventId и UserId)
        [HttpGet("{eventId}/{userId}")]
        public async Task<ActionResult<ConnectorEventUser>> GetByCompositeKey(int eventId, int userId)
        {
            var record = await _repository.GetByCompositeKeyAsync(eventId, userId);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        // Получить все записи для конкретного пользователя по UserId
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ConnectorEventUser>>> GetAllByUserId(int userId)
        {
            var records = await _repository.GetAllByUserIdAsync(userId);
            return Ok(records);
        }

        // Получить все записи для конкретного события по EventId
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<ConnectorEventUser>>> GetAllByEventId(int eventId)
        {
            var records = await _repository.GetAllByEventIdAsync(eventId);
            return Ok(records);
        }

        // Добавить новую запись
        [HttpPost]
        public async Task<ActionResult<ConnectorEventUser>> Create([FromBody] ConnectorEventUser record)
        {
            await _repository.AddAsync(record);
            return CreatedAtAction(nameof(GetByCompositeKey), new { eventId = record.EventId, userId = record.UserId }, record);
        }

        // Обновить запись
        [HttpPut("{eventId}/{userId}")]
        public async Task<IActionResult> Update(int eventId, int userId, [FromBody] ConnectorEventUser record)
        {
            if (eventId != record.EventId || userId != record.UserId)
            {
                return BadRequest("Несоответствие составного ключа.");
            }

            await _repository.UpdateAsync(record);
            return NoContent();
        }

        [HttpDelete("{eventId}/{userId}")]
        public async Task<IActionResult> Delete(int eventId, int userId)
        {
            await _repository.DeleteByCompositeKeyAsync(eventId, userId);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<ConnectorEventUser>>> GetPaged(int pageNumber = 1, int pageSize = 10)
        {
            if(pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Количество записей и номер страницы должны быть больше нуля");
            }
            var records = await _repository.GetPagedAsync(pageNumber, pageSize);
            return Ok(records);
        }
    }
}