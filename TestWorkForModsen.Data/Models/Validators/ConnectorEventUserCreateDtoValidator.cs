﻿using FluentValidation;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Data.Models.Validators
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
                throw new ValidationException(result.Errors);
            }
        }
    }
}
