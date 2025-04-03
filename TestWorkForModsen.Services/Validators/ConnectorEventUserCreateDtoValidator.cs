using FluentValidation;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Validators
{
    public class ConnectorEventUserCreateDtoValidator : BaseValidator<ConnectorEventUserCreateDto>
    {
        public ConnectorEventUserCreateDtoValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
