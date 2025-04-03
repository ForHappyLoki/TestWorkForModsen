using FluentValidation;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Validators
{
    public class ConnectorEventUserCreateDtoValidator : AbstractValidator<ConnectorEventUserCreateDto>
    {
        public ConnectorEventUserCreateDtoValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
        public async Task ValidateAndThrowAsync(ConnectorEventUserCreateDto dto)
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
