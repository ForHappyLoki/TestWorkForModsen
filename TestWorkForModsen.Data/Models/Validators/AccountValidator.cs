using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Data.Models.Validators
{
    public class AccountValidator : AbstractValidator<AccountDto>
    {
        public AccountValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .MaximumLength(50).WithMessage("Role must not exceed 50 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0");
        }
        public async Task ValidateAndThrowAsync(AccountDto dto)
        {
            var result = await ValidateAsync(dto);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
