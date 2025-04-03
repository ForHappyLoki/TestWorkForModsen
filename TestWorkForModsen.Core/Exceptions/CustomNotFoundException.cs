using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Core.Exceptions
{
    public class CustomNotFoundException : ApiException
    {
        public CustomNotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}
