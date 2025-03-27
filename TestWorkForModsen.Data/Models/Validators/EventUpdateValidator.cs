﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Data.Models.Validators
{
    public class EventUpdateValidator : AbstractValidator<EventUpdateDto>
    {
        public EventUpdateValidator()
        {
            // Правильный способ включения правил валидации
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название обязательно")
                .MaximumLength(100).WithMessage("Максимальная длина названия - 100 символов");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Максимальная длина описания - 500 символов");

            RuleFor(x => x.DateTime)
                .GreaterThan(DateTime.Now).WithMessage("Дата должна быть в будущем");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Категория обязательна");

            RuleFor(x => x.MaxParticipants)
                .Matches(@"^\d+$").WithMessage("Укажите число участников");

            // Дополнительное правило для ID
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Неверный ID");
        }
    }
}
