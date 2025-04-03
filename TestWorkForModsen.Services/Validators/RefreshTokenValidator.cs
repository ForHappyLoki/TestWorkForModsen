using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Validators
{
    public class RefreshTokenValidator : BaseValidator<RefreshToken>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Токен не может быть пустым")
                .Length(32, 512).WithMessage("Токен должен быть от 32 до 512 символов");

            RuleFor(x => x.ExpiryTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Срок действия токена истек");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("Некорректный ID аккаунта");
        }
    }
}
