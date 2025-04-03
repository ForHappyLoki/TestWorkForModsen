using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Core.Exceptions;

namespace TestWorkForModsen.Services.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        public async Task ValidateAndThrowAsync(T dto)
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
