using Microsoft.AspNetCore.Mvc;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace TestWork_Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController(IRepository<User> repository) : Controller
    {
        private readonly IRepository<User> _userRepository = repository;

        // Получить всех пользователей
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }
        // Получить пользователя по ID
        [HttpGet("id/{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var account = await _userRepository.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        // Получить пользователя по email
        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetByEmail(string email)
        {
            var account = await _userRepository.GetByEmailAsync(email);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        // Создать нового пользователя
        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // Обновить пользователя
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            await _userRepository.UpdateAsync(account);
            return NoContent();
        }

        // Удалить пользователя
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userRepository.DeleteAsync(id);
            return NoContent();
        }

        // Пагинация
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<User>>> GetPaged(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Номер страницы и размер страницы должны быть больше 0.");
            }

            var accounts = await _userRepository.GetPagedAsync(pageNumber, pageSize);
            return Ok(accounts);
        }
    }
}
