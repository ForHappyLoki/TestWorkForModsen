using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Models.Validators;
using TestWorkForModsen.Services.Services;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAll()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<AccountResponseDto>> GetById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            return account == null ? NotFound() : Ok(account);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<AccountResponseDto>> GetByEmail(string email)
        {
            var account = await _accountService.GetAccountByEmailAsync(email);
            return account == null ? NotFound() : Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult<AccountResponseDto>> Create([FromBody] AccountDto accountDto)
        {
            var validationResult = await _validator.ValidateAsync(accountDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var account = await _accountService.CreateAccountAsync(accountDto);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AccountDto accountDto)
        {
            var validationResult = await _validator.ValidateAsync(accountDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                await _accountService.UpdateAccountAsync(id, accountDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _accountService.DeleteAccountAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetPaged(
            [FromQuery] PaginationDto paginationDto)
        {
            var validationResult = await _paginationValidator.ValidateAsync(new ValidationContext<PaginationDto>(paginationDto));
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var accounts = await _accountService.GetPagedAccountsAsync(paginationDto);
            return Ok(accounts);
        }
    }
}