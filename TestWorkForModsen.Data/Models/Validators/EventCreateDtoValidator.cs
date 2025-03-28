using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Data.Models.Validators
{
    public class EventCreateDtoValidator : AbstractValidator<EventCreateDto>
    {
        public EventCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название события обязательно")
                .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно")
                .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов");

            RuleFor(x => x.DateTime)
                .NotEmpty().WithMessage("Дата и время обязательны")
                .GreaterThan(DateTime.Now).WithMessage("Дата должна быть в будущем");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Категория обязательна")
                .MaximumLength(50).WithMessage("Категория не должна превышать 50 символов");

            RuleFor(x => x.MaxParticipants)
                .NotEmpty().WithMessage("Количество участников обязательно")
                .Matches(@"^\d+$").WithMessage("Должно быть числовое значение")
                .Must(x => int.TryParse(x, out var val) && val > 0)
                .WithMessage("Количество участников должно быть положительным числом");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Изображение обязательно")
                .Must(x => x.Length <= 5 * 1024 * 1024) 
                .WithMessage("Размер изображения не должен превышать 5MB")
                .Must(x => IsValidImageType(x))
                .WithMessage("Неподдерживаемый формат изображения");
        }
        public async Task ValidateAndThrowAsync(EventCreateDto dto)
        {
            var result = await ValidateAsync(dto);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
        private bool IsValidImageType(byte[] image)
        {
            if (image.Length < 4) return false;

            if (image[0] == 0xFF && image[1] == 0xD8 && image[2] == 0xFF)
                return true;

            if (image[0] == 0x89 && image[1] == 0x50 &&
                image[2] == 0x4E && image[3] == 0x47)
                return true;

            return false;
        }
    }
}
