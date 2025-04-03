using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Core.Exceptions
{
    public class CustomTokenGenerationException : ApiException
    {
        public CustomTokenGenerationException(string message, Exception innerException)
            : base(message, HttpStatusCode.InternalServerError)
        {
        }
    }
}
