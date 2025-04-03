using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Services.Services;
using Microsoft.AspNetCore.Authorization;

namespace TestWorkForModsen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IValidator<AccountDto> _validator;
        private readonly IValidator<PaginationDto> _paginationValidator;
        public AccountApiController(
            IAccountService accountService,
            IValidator<AccountDto> validator,
            IValidator<PaginationDto> paginationValidator)
        {
            _accountService = accountService;
            _validator = validator;
            _paginationValidator = paginationValidator;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAll()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("id/{id}")]
        public async Task<ActionResult<AccountResponseDto>> GetById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            return Ok(account);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("email/{email}")]
        public async Task<ActionResult<AccountResponseDto>> GetByEmail(string email)
        {
            var account = await _accountService.GetAccountByEmailAsync(email);
            return Ok(account);
        }
        [HttpPost]
        public async Task<ActionResult<AccountResponseDto>> Create([FromBody] AccountDto accountDto)
        {
            await _validator.ValidateAndThrowAsync(accountDto);
            var account = await _accountService.CreateAccountAsync(accountDto);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AccountDto accountDto)
        {
            await _validator.ValidateAndThrowAsync(accountDto);
            await _accountService.UpdateAccountAsync(accountDto);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetPaged(
            [FromQuery] PaginationDto paginationDto)
        {
            await _paginationValidator.ValidateAndThrowAsync(paginationDto);
            var accounts = await _accountService.GetPagedAccountsAsync(paginationDto);
            return Ok(accounts);
        }
    }
}