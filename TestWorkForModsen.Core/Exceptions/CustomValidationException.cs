using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Core.Exceptions
{
    public class CustomValidationException : ApiException
    {
        public IDictionary<string, string[]> Errors { get; }

        public CustomValidationException(IDictionary<string, string[]> errors)
            : base("Ошибка валидации", HttpStatusCode.UnprocessableEntity)
        {
            Errors = errors;
        }
    }
}
