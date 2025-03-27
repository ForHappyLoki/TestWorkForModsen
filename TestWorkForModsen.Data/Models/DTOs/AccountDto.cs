using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Data.Models.DTOs
{
    public class AccountDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
    }
}
