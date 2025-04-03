using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Validators
{
    public class UserValidator : AbstractValidator<UserCreateDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно")
                .MaximumLength(50).WithMessage("Максимальная длина имени - 50 символов");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Фамилия обязательна")
                .MaximumLength(50).WithMessage("Максимальная длина фамилии - 50 символов");

            RuleFor(x => x.Birthday)
                .LessThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-14)))
                .WithMessage("Пользователь должен быть старше 14 лет");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email")
                .MaximumLength(100).WithMessage("Максимальная длина email - 100 символов");
        }
        public async Task ValidateAndThrowAsync(UserCreateDto dto)
        {
            var result = await ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                throw new CustomValidationException(errors);
            }
        }
    }
}
