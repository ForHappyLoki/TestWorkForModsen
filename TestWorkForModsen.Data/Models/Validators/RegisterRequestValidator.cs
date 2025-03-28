using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Data.Models.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
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
                .EmailAddress().WithMessage("Некорректный формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов")
                .Equal(x => x.ConfirmPassword).WithMessage("Пароли не совпадают");
        }
        public async Task ValidateAndThrowAsync(RegisterRequestDto dto)
        {
            var result = await ValidateAsync(dto);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
