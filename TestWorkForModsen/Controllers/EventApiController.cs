using Microsoft.AspNetCore.Mvc;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace TestWork_Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventApiController(IRepository<Event> repository) : Controller
    {
        private readonly IRepository<Event> _eventRepository = repository;

        // Получить всех события
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAll()
        {
            var _events = await _eventRepository.GetAllAsync();
            return Ok(_events);
        }
        // Получить событие по ID
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Event>> GetById(int id)
        {
            var _event = await _eventRepository.GetByIdAsync(id);
            if (_event == null)
            {
                return NotFound();
            }
            return Ok(_event);
        }

        // Создать новое событие
        [HttpPost]
        public async Task<ActionResult<Event>> Create([FromBody] Event _event)
        {
            await _eventRepository.AddAsync(_event);
            return CreatedAtAction(nameof(GetById), new { id = _event.Id }, _event);
        }

        // Обновить событие
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Event _event)
        {
            if (id != _event.Id)
            {
                return BadRequest();
            }

            await _eventRepository.UpdateAsync(_event);
            return NoContent();
        }

        // Удалить событие
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventRepository.DeleteAsync(id);
            return NoContent();
        }

        // Пагинация
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Event>>> GetPaged(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Номер страницы и размер страницы должны быть больше 0.");
            }

            var _events = await _eventRepository.GetPagedAsync(pageNumber, pageSize);
            return Ok(_events);
        }
    }
}
