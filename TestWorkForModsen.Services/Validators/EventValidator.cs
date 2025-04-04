﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Validators
{
    public class EventValidator : BaseValidator<EventDto>
    {
        public EventValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название события обязательно")
                .MaximumLength(100).WithMessage("Максимальная длина названия - 100 символов");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Максимальная длина описания - 500 символов");

            RuleFor(x => x.DateTime)
                .GreaterThan(DateTime.Now).WithMessage("Дата события должна быть в будущем");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Категория обязательна");

            RuleFor(x => x.MaxParticipants)
                .Matches(@"^\d+$").WithMessage("Укажите число участников");
        }
    }
}
