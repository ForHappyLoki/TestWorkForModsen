using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Data.Models.DTOs
{
    public class UserUpdateDto : UserCreateDto
    {
        public int Id { get; set; }
    }
}
