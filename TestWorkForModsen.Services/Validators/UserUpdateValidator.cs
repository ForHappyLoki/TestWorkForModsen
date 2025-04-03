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
    public class UserUpdateValidator : BaseValidator<UserUpdateDto>
    {
        public UserUpdateValidator()
        {
            Include(new UserValidator());
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Неверный ID");
        }
    }
}
