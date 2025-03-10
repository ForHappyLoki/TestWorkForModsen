using Microsoft.AspNetCore.Mvc;
using TestWork_Events.Data;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace TestWork_Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController(IRepository<Account> repository) : ControllerBase
    {
        private readonly IRepository<Account> _accountRepository = repository;

        // Получить все аккаунты
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return Ok(accounts);
        }
        // Получить аккаунт по ID
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Account>> GetById(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        // Получить аккаунт по email
        [HttpGet("email/{email}")]
        public async Task<ActionResult<Account>> GetByEmail(string email)
        {
            var account = await _accountRepository.GetByEmailAsync(email);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        // Создать новый аккаунт
        [HttpPost]
        public async Task<ActionResult<Account>> Create([FromBody] Account account)
        {
            await _accountRepository.AddAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        // Обновить аккаунт
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            await _accountRepository.UpdateAsync(account);
            return NoContent();
        }

        // Удалить аккаунт
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _accountRepository.DeleteAsync(id);
            return NoContent();
        }

        // Пагинация
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Account>>> GetPaged(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Номер страницы и размер страницы должны быть больше 0.");
            }

            var accounts = await _accountRepository.GetPagedAsync(pageNumber, pageSize);
            return Ok(accounts);
        }
    }
}