﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Data.Models.Validators
{
    public class ConnectorEventUserDtoValidator : AbstractValidator<ConnectorEventUserDto>
    {
        public ConnectorEventUserDtoValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
