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
    public class ConnectorEventUserDtoValidator : BaseValidator<ConnectorEventUserDto>
    {
        public ConnectorEventUserDtoValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
